// Tipos compartilhados

export interface Cliente {
  codCliente: number;
  cnpj: string;
  nome: string;
  email: string;
  dataCadastro: string;
}

export interface CriarClienteDTO {
  cnpj: string;
  nome: string;
  email: string;
}

export interface Produto {
  codProduto: number;
  nome: string;
  preco: number;
  estoque: number;
}

export interface CriarProdutoDTO {
  nome: string;
  preco: number;
  estoque: number;
}

export interface Pedido {
  codPedido: number;
  clienteNome: string;
  clienteCNPJ: string;
  dataPedido: string;
  valorTotal: number;
  itens: ItemPedido[];
}

export interface ItemPedido {
  codProduto: number;
  nomeProduto: string;
  quantidade: number;
  precoUnitario: number;
  subtotal: number;
}

export interface CriarPedidoDTO {
  cnpj: string;
}

export interface AdicionarItemDTO {
  codProduto: number;
  quantidade: number;
}

export interface ApiError {
  error: {
    message: string;
    detail: string;
  };
}