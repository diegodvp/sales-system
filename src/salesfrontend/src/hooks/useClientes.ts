'use client';

import { useState, useEffect, useCallback } from 'react';
import { api } from '@/services/api';
import { Cliente } from '@/types';

export function useClientes() {
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchClientes = useCallback(async () => {
    try {
      setLoading(true);
      const data = await api.getClientes();
      setClientes(data);
      setError(null);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchClientes();
  }, [fetchClientes]);

  return { clientes, loading, error, refetch: fetchClientes };
}