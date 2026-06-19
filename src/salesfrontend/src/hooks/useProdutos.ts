'use client';

import { useState, useEffect, useCallback } from 'react';
import { api } from '@/services/api';
import { Produto } from '@/types';

export function useProdutos() {
  const [produtos, setProdutos] = useState<Produto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchProdutos = useCallback(async () => {
    try {
      setLoading(true);
      const data = await api.getProdutos();
      setProdutos(data);
      setError(null);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchProdutos();
  }, [fetchProdutos]);

  return { produtos, loading, error, refetch: fetchProdutos };
}