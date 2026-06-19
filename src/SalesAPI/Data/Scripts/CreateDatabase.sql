-- Verificar se o banco existe, se não, criar
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SalesDB')
BEGIN
    CREATE DATABASE SalesDB;
END
GO

USE SalesDB;
GO

-- Criar tabelas
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cliente' AND xtype='U')
BEGIN
    CREATE TABLE Cliente (
        CodCliente INT IDENTITY(1,1) PRIMARY KEY,
        CNPJ VARCHAR(14) NOT NULL UNIQUE,
        Nome VARCHAR(100) NOT NULL,
        Email VARCHAR(100),
        DataCadastro DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Produto' AND xtype='U')
BEGIN
    CREATE TABLE Produto (
        CodProduto INT IDENTITY(1,1) PRIMARY KEY,
        Nome VARCHAR(100) NOT NULL,
        Preco DECIMAL(10,2) NOT NULL,
        Estoque INT NOT NULL DEFAULT 0
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Pedido' AND xtype='U')
BEGIN
    CREATE TABLE Pedido (
        CodPedido INT IDENTITY(1,1) PRIMARY KEY,
        CodCliente INT NOT NULL,
        DataPedido DATETIME NOT NULL DEFAULT GETDATE(),
        ValorTotal DECIMAL(10,2) NOT NULL DEFAULT 0,
        FOREIGN KEY (CodCliente) REFERENCES Cliente(CodCliente)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ItensPedido' AND xtype='U')
BEGIN
    CREATE TABLE ItensPedido (
        CodPedido INT NOT NULL,
        CodProduto INT NOT NULL,
        Quantidade INT NOT NULL,
        PrecoUnitario DECIMAL(10,2) NOT NULL,
        PRIMARY KEY (CodPedido, CodProduto),
        FOREIGN KEY (CodPedido) REFERENCES Pedido(CodPedido),
        FOREIGN KEY (CodProduto) REFERENCES Produto(CodProduto)
    );
END
GO

-- Índices para melhor performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedido_CodCliente')
    CREATE INDEX IX_Pedido_CodCliente ON Pedido(CodCliente);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pedido_DataPedido')
    CREATE INDEX IX_Pedido_DataPedido ON Pedido(DataPedido);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ItensPedido_CodPedido')
    CREATE INDEX IX_ItensPedido_CodPedido ON ItensPedido(CodPedido);

-- Clientes
IF NOT EXISTS (SELECT 1 FROM Cliente WHERE CNPJ = '11222333000181')
BEGIN
    INSERT INTO Cliente (CNPJ, Nome, Email, DataCadastro) 
    VALUES ('11222333000181', 'João da Silva', 'joao.silva@email.com', GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE CNPJ = '44555666000192')
BEGIN
    INSERT INTO Cliente (CNPJ, Nome, Email, DataCadastro) 
    VALUES ('44555666000192', 'Maria Oliveira', 'maria.oliveira@email.com', GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM Cliente WHERE CNPJ = '77888999000109')
BEGIN
    INSERT INTO Cliente (CNPJ, Nome, Email, DataCadastro) 
    VALUES ('77888999000109', 'Empresa ABC Ltda', 'contato@empresaabc.com.br', GETDATE());
END
GO

-- Produtos
IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = 'Monitor')
BEGIN
    INSERT INTO Produto (Nome, Preco, Estoque) 
    VALUES ('Monitor', 1500.00, 5);
END
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = 'Teclado USB')
BEGIN
    INSERT INTO Produto (Nome, Preco, Estoque) 
    VALUES ('Teclado USB', 150.00, 0);
END
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = 'Mouse')
BEGIN
    INSERT INTO Produto (Nome, Preco, Estoque) 
    VALUES ('Mouse', 80.00, 10);
END
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = 'Headset')
BEGIN
    INSERT INTO Produto (Nome, Preco, Estoque) 
    VALUES ('Headset', 250.00, 3);
END
GO

IF NOT EXISTS (SELECT 1 FROM Produto WHERE Nome = 'Webcam')
BEGIN
    INSERT INTO Produto (Nome, Preco, Estoque) 
    VALUES ('Webcam', 200.00, 7);
END
GO

PRINT 'Banco de dados SalesDB criado com sucesso!';
GO