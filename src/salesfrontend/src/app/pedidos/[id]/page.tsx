'use client';

import { useParams, useRouter } from 'next/navigation';
import { usePedido } from '@/hooks/usePedidos';
import { formatDate, formatCurrency, formatCNPJ } from '@/utils/format';
import Loading from '@/components/ui/Loading';
import Button from '@/components/ui/Button';

export default function DetalhesPedidoPage() {
  const { id } = useParams();
  const router = useRouter();
  const { pedido, loading } = usePedido(Number(id));

  if (loading) return <Loading />;
  if (!pedido) return (
    <div className="text-center py-8">
      <p className="text-gray-500">Pedido não encontrado</p>
      <Button variant="secondary" onClick={() => router.push('/')} className="mt-4">
        Voltar
      </Button>
    </div>
  );

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Pedido #{pedido.codPedido}</h2>
        <Button variant="secondary" onClick={() => router.push('/')}>
          Voltar
        </Button>
      </div>

      <div className="bg-white rounded-lg shadow p-6 mb-6">
        <h3 className="text-lg font-semibold mb-4 text-gray-800">Dados do Pedido</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <p className="text-sm text-gray-600">Cliente</p>
            <p className="font-medium text-lg">{pedido.clienteNome}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">CNPJ</p>
            <p className="font-medium">{formatCNPJ(pedido.clienteCNPJ)}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">Data do Pedido</p>
            <p className="font-medium">{formatDate(pedido.dataPedido)}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">Valor Total</p>
            <p className="font-bold text-2xl text-blue-600">{formatCurrency(pedido.valorTotal)}</p>
          </div>
        </div>
      </div>

      <div className="bg-white rounded-lg shadow p-6">
        <h3 className="text-lg font-semibold mb-4 text-gray-800">
          Itens do Pedido ({pedido.itens.length})
        </h3>
        
        {pedido.itens.length === 0 ? (
          <p className="text-gray-500 text-center py-4">Nenhum item neste pedido</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b-2 border-gray-200">
                  <th className="text-left py-3 px-4">Produto</th>
                  <th className="text-center py-3 px-4">Quantidade</th>
                  <th className="text-right py-3 px-4">Preço Unitário</th>
                  <th className="text-right py-3 px-4">Subtotal</th>
                </tr>
              </thead>
              <tbody>
                {pedido.itens.map((item) => (
                  <tr key={item.codProduto} className="border-b border-gray-100 hover:bg-gray-50">
                    <td className="py-3 px-4 font-medium">{item.nomeProduto}</td>
                    <td className="text-center py-3 px-4">{item.quantidade}</td>
                    <td className="text-right py-3 px-4">{formatCurrency(item.precoUnitario)}</td>
                    <td className="text-right py-3 px-4 font-semibold">{formatCurrency(item.subtotal)}</td>
                  </tr>
                ))}
                <tr className="bg-gray-50 font-bold">
                  <td colSpan={3} className="text-right py-3 px-4">TOTAL</td>
                  <td className="text-right py-3 px-4 text-blue-600 text-lg">
                    {formatCurrency(pedido.valorTotal)}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}