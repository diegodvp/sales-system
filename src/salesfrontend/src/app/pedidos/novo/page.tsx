'use client';

import { useState, useEffect } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { api } from '@/services/api';
import { usePedido } from '@/hooks/usePedidos';
import { useProdutos } from '@/hooks/useProdutos';
import { formatCurrency } from '@/utils/format';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Loading from '@/components/ui/Loading';

export default function NovoPedidoPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const pedidoId = Number(searchParams.get('id'));
  
  const { pedido, loading: loadingPedido, refetch } = usePedido(pedidoId);
  const { produtos, loading: loadingProdutos } = useProdutos();
  
  const [quantidades, setQuantidades] = useState<Record<number, number>>({});
  const [adding, setAdding] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleAddItem = async (codProduto: number) => {
    const quantidade = quantidades[codProduto] || 1;
    
    try {
      setAdding(codProduto);
      setError(null);
      await api.adicionarItem(pedidoId, { codProduto, quantidade });
      setQuantidades({ ...quantidades, [codProduto]: 0 });
      refetch();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setAdding(null);
    }
  };

  const handleUpdateQuantidade = async (codProduto: number, novaQuantidade: number) => {
    try {
      await api.atualizarItem(pedidoId, codProduto, { quantidade: novaQuantidade });
      refetch();
    } catch (err: any) {
      setError(err.message);
    }
  };

  const handleRemoverItem = async (codProduto: number) => {
    try {
      await api.removerItem(pedidoId, codProduto);
      refetch();
    } catch (err: any) {
      setError(err.message);
    }
  };

  if (loadingPedido) return <Loading />;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-gray-800">
          Pedido #{pedido?.codPedido}
        </h2>
        <div className="text-xl font-semibold text-blue-600">
          Total: {formatCurrency(pedido?.valorTotal || 0)}
        </div>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
          <button onClick={() => setError(null)} className="float-right font-bold">&times;</button>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Produtos Disponíveis */}
        <div>
          <h3 className="text-lg font-semibold mb-4">Produtos Disponíveis</h3>
          <div className="space-y-3">
            {produtos.map((produto) => (
              <div key={produto.codProduto} className="bg-white p-4 rounded-lg shadow">
                <div className="flex justify-between items-start mb-2">
                  <div>
                    <h4 className="font-medium">{produto.nome}</h4>
                    <p className="text-sm text-gray-600">
                      {formatCurrency(produto.preco)} | Estoque: {produto.estoque}
                    </p>
                  </div>
                </div>
                <div className="flex gap-2">
                  <input
                    type="number"
                    min="1"
                    max={produto.estoque}
                    value={quantidades[produto.codProduto] || 1}
                    onChange={(e) => setQuantidades({
                      ...quantidades,
                      [produto.codProduto]: Number(e.target.value)
                    })}
                    className="w-20 px-2 py-1 border rounded"
                    disabled={produto.estoque === 0}
                  />
                  <Button
                    onClick={() => handleAddItem(produto.codProduto)}
                    disabled={produto.estoque === 0 || adding === produto.codProduto}
                    variant={produto.estoque === 0 ? 'secondary' : 'primary'}
                    className="flex-1"
                  >
                    {produto.estoque === 0
                      ? 'Sem Estoque'
                      : adding === produto.codProduto
                      ? 'Adicionando...'
                      : 'Adicionar'}
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Itens do Pedido */}
        <div>
          <h3 className="text-lg font-semibold mb-4">Itens do Pedido</h3>
          {pedido?.itens.length === 0 ? (
            <p className="text-gray-500">Nenhum item adicionado</p>
          ) : (
            <div className="space-y-3">
              {pedido?.itens.map((item) => (
                <div key={item.codProduto} className="bg-white p-4 rounded-lg shadow">
                  <div className="flex justify-between items-start mb-2">
                    <div>
                      <h4 className="font-medium">{item.nomeProduto}</h4>
                      <p className="text-sm text-gray-600">
                        {formatCurrency(item.precoUnitario)} x {item.quantidade}
                      </p>
                    </div>
                    <p className="font-semibold">{formatCurrency(item.subtotal)}</p>
                  </div>
                  <div className="flex gap-2 items-center">
                    <input
                      type="number"
                      min="0"
                      value={item.quantidade}
                      onChange={(e) => handleUpdateQuantidade(item.codProduto, Number(e.target.value))}
                      className="w-20 px-2 py-1 border rounded"
                    />
                    <Button
                      onClick={() => handleRemoverItem(item.codProduto)}
                      variant="danger"
                    >
                      Remover
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      <div className="mt-6">
        <Button variant="secondary" onClick={() => router.push('/')}>
          Voltar
        </Button>
      </div>
    </div>
  );
}