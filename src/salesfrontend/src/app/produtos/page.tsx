'use client';

import { useState } from 'react';
import { api } from '@/services/api';
import { useProdutos } from '@/hooks/useProdutos';
import { formatCurrency } from '@/utils/format';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Loading from '@/components/ui/Loading';

export default function ProdutosPage() {
  const { produtos, loading, refetch } = useProdutos();
  const [nome, setNome] = useState('');
  const [preco, setPreco] = useState('');
  const [estoque, setEstoque] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!nome || !preco || !estoque) {
      setError('Todos os campos são obrigatórios');
      return;
    }

    try {
      await api.criarProduto({
        nome,
        preco: Number(preco),
        estoque: Number(estoque),
      });
      
      setNome('');
      setPreco('');
      setEstoque('');
      setSuccess('Produto cadastrado com sucesso!');
      refetch();
      
      setTimeout(() => setSuccess(''), 3000);
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Cadastro de Produtos</h2>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold mb-4">Novo Produto</h3>
          
          {error && (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
              {error}
            </div>
          )}
          
          {success && (
            <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4">
              {success}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <Input
              label="Nome do Produto"
              name="nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              placeholder="Ex: Monitor"
            />
            <Input
              label="Preço (R$)"
              name="preco"
              type="number"
              value={preco}
              onChange={(e) => setPreco(e.target.value)}
              placeholder="0.00"
            />
            <Input
              label="Quantidade em Estoque"
              name="estoque"
              type="number"
              value={estoque}
              onChange={(e) => setEstoque(e.target.value)}
              placeholder="0"
            />
            <Button type="submit" className="w-full">
              Cadastrar Produto
            </Button>
          </form>
        </div>

        <div>
          <h3 className="text-lg font-semibold mb-4">Produtos Cadastrados</h3>
          {loading ? (
            <Loading />
          ) : (
            <div className="space-y-3">
              {produtos.map((produto) => (
                <div key={produto.codProduto} className="bg-white p-4 rounded-lg shadow">
                  <div className="flex justify-between items-start">
                    <div>
                      <h4 className="font-medium">{produto.nome}</h4>
                      <p className="text-sm text-gray-600">
                        {formatCurrency(produto.preco)}
                      </p>
                    </div>
                    <span className={`px-2 py-1 rounded text-sm font-medium ${
                      produto.estoque > 0 
                        ? 'bg-green-100 text-green-800' 
                        : 'bg-red-100 text-red-800'
                    }`}>
                      Estoque: {produto.estoque}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}