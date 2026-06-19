'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { usePedidos } from '@/hooks/usePedidos';
import { api } from '@/services/api';
import { formatDate, formatCurrency, formatCNPJ } from '@/utils/format';
import { isValidCNPJ } from '@/utils/validators';
import Table from '@/components/ui/Table';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Modal from '@/components/ui/Modal';
import Loading from '@/components/ui/Loading';

export default function HomePage() {
  const router = useRouter();
  const { pedidos, loading, refetch } = usePedidos();
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [cnpj, setCnpj] = useState('');
  const [cnpjError, setCnpjError] = useState('');
  const [creating, setCreating] = useState(false);
  
  const [filters, setFilters] = useState({
    dataInicio: '',
    dataFim: '',
    clienteNome: '',
  });

  const handleFilter = () => {
    const activeFilters: any = {};
    if (filters.dataInicio) activeFilters.dataInicio = filters.dataInicio;
    if (filters.dataFim) activeFilters.dataFim = filters.dataFim;
    if (filters.clienteNome) activeFilters.clienteNome = filters.clienteNome;
    refetch(activeFilters);
  };

  const handleCreatePedido = async () => {
    setCnpjError('');
    
    if (!cnpj) {
      setCnpjError('CNPJ é obrigatório');
      return;
    }

    if (!isValidCNPJ(cnpj)) {
      setCnpjError('CNPJ inválido');
      return;
    }

    try {
      setCreating(true);
      const pedido = await api.criarPedido({ cnpj });
      setIsModalOpen(false);
      setCnpj('');
      router.push(`/pedidos/novo?id=${pedido.codPedido}`);
    } catch (err: any) {
      setCnpjError(err.message);
    } finally {
      setCreating(false);
    }
  };

  const columns = [
    { header: 'Pedido', accessor: 'codPedido' as const },
    { header: 'Cliente', accessor: 'clienteNome' as const },
    { header: 'CNPJ', accessor: (pedido: any) => formatCNPJ(pedido.clienteCNPJ) },
    { header: 'Data', accessor: (pedido: any) => formatDate(pedido.dataPedido) },
    { header: 'Valor Total', accessor: (pedido: any) => formatCurrency(pedido.valorTotal) },
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Pedidos</h2>
        <Button onClick={() => setIsModalOpen(true)}>Novo Pedido</Button>
      </div>

      {/* Filtros */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <Input
            label="Data Início"
            name="dataInicio"
            type="date"
            value={filters.dataInicio}
            onChange={(e) => setFilters({ ...filters, dataInicio: e.target.value })}
          />
          <Input
            label="Data Fim"
            name="dataFim"
            type="date"
            value={filters.dataFim}
            onChange={(e) => setFilters({ ...filters, dataFim: e.target.value })}
          />
          <Input
            label="Cliente"
            name="clienteNome"
            value={filters.clienteNome}
            onChange={(e) => setFilters({ ...filters, clienteNome: e.target.value })}
            placeholder="Nome do cliente"
          />
          <div className="flex items-end">
            <Button onClick={handleFilter} variant="secondary" className="w-full">
              Filtrar
            </Button>
          </div>
        </div>
      </div>

      {/* Tabela */}
      {loading ? (
        <Loading />
      ) : (
        <Table
          columns={columns}
          data={pedidos}
          onRowClick={(pedido) => router.push(`/pedidos/${pedido.codPedido}`)}
          emptyMessage="Nenhum pedido encontrado"
        />
      )}

      {/* Modal Novo Pedido */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => {
          setIsModalOpen(false);
          setCnpj('');
          setCnpjError('');
        }}
        title="Novo Pedido"
      >
        <p className="text-sm text-gray-600 mb-4">
          Informe o CNPJ do cliente para criar um novo pedido.
        </p>
        <Input
          label="CNPJ do Cliente"
          name="cnpj"
          value={cnpj}
          onChange={(e) => setCnpj(e.target.value)}
          error={cnpjError}
          placeholder="00.000.000/0000-00"
          mask="cnpj"
        />
        <div className="flex justify-end gap-2 mt-6">
          <Button
            variant="secondary"
            onClick={() => {
              setIsModalOpen(false);
              setCnpj('');
              setCnpjError('');
            }}
          >
            Cancelar
          </Button>
          <Button onClick={handleCreatePedido} disabled={creating}>
            {creating ? 'Criando...' : 'Criar Pedido'}
          </Button>
        </div>
      </Modal>
    </div>
  );
}