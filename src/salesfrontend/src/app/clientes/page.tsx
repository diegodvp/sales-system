'use client';

import { useState } from 'react';
import { api } from '@/services/api';
import { useClientes } from '@/hooks/useClientes';
import { formatCNPJ, formatDate } from '@/utils/format';
import { isValidCNPJ } from '@/utils/validators';
import Button from '@/components/ui/Button';
import Input from '@/components/ui/Input';
import Loading from '@/components/ui/Loading';

export default function ClientesPage() {
  const { clientes, loading, refetch } = useClientes();
  const [cnpj, setCnpj] = useState('');
  const [nome, setNome] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [cnpjError, setCnpjError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setCnpjError('');

    if (!cnpj || !nome) {
      setError('CNPJ e Nome são obrigatórios');
      return;
    }

    if (!isValidCNPJ(cnpj)) {
      setCnpjError('CNPJ inválido');
      return;
    }

    try {
      await api.criarCliente({
        cnpj,
        nome,
        email,
      });
      
      setCnpj('');
      setNome('');
      setEmail('');
      setSuccess('Cliente cadastrado com sucesso!');
      refetch();
      
      setTimeout(() => setSuccess(''), 3000);
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div>
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Cadastro de Clientes</h2>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold mb-4">Novo Cliente</h3>
          
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
              label="CNPJ"
              name="cnpj"
              value={cnpj}
              onChange={(e) => setCnpj(e.target.value)}
              error={cnpjError}
              placeholder="00.000.000/0000-00"
              mask="cnpj"
            />
            <Input
              label="Nome"
              name="nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              placeholder="Nome do cliente"
            />
            <Input
              label="E-mail"
              name="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="email@exemplo.com"
            />
            <Button type="submit" className="w-full">
              Cadastrar Cliente
            </Button>
          </form>
        </div>

        <div>
          <h3 className="text-lg font-semibold mb-4">Clientes Cadastrados</h3>
          {loading ? (
            <Loading />
          ) : (
            <div className="space-y-3">
              {clientes.map((cliente) => (
                <div key={cliente.codCliente} className="bg-white p-4 rounded-lg shadow">
                  <h4 className="font-medium text-lg">{cliente.nome}</h4>
                  <p className="text-sm text-gray-600">CNPJ: {formatCNPJ(cliente.cnpj)}</p>
                  {cliente.email && (
                    <p className="text-sm text-gray-600">Email: {cliente.email}</p>
                  )}
                  <p className="text-xs text-gray-400 mt-1">
                    Cadastrado em: {formatDate(cliente.dataCadastro)}
                  </p>
                </div>
              ))}
              {clientes.length === 0 && (
                <p className="text-gray-500 text-center py-4">Nenhum cliente cadastrado</p>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}