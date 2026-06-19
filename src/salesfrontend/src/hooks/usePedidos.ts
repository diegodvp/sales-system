'use client';

import { useState, useEffect, useCallback } from 'react';
import { api } from '@/services/api';
import { Pedido } from '@/types';

export function usePedidos() {
  const [pedidos, setPedidos] = useState<Pedido[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchPedidos = useCallback(async (filters?: any) => {
    try {
      setLoading(true);
      const data = await api.getPedidos(filters);
      setPedidos(data);
      setError(null);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchPedidos();
  }, [fetchPedidos]);

  return { pedidos, loading, error, refetch: fetchPedidos };
}

export function usePedido(id: number) {
  const [pedido, setPedido] = useState<Pedido | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchPedido = useCallback(async () => {
    try {
      setLoading(true);
      const data = await api.getPedidoById(id);
      setPedido(data);
      setError(null);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    fetchPedido();
  }, [fetchPedido]);

  return { pedido, loading, error, refetch: fetchPedido };
}