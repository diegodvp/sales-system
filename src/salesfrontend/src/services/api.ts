// Camada de network reutilizável

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:5001/api';

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  private async request<T>(
    endpoint: string,
    options?: RequestInit
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
    
    const config: RequestInit = {
      headers: {
        'Content-Type': 'application/json',
      },
      ...options,
    };

    const response = await fetch(url, config);

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error?.message || 'Erro na requisição');
    }

    if (response.status === 204) {
      return null as T;
    }

    return response.json();
  }

  // Clientes
  async getClientes() {
    return this.request<any[]>('/Cliente');
  }

  async getClienteById(id: number) {
    return this.request<any>(`/Cliente/${id}`);
  }

  async criarCliente(data: any) {
    return this.request<any>('/Cliente', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  // Produtos
  async getProdutos() {
    return this.request<any[]>('/Produto');
  }

  async criarProduto(data: any) {
    return this.request<any>('/Produto', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  // Pedidos
  async getPedidos(filters?: { dataInicio?: string; dataFim?: string; clienteNome?: string }) {
    const params = new URLSearchParams();
    if (filters?.dataInicio) params.append('dataInicio', filters.dataInicio);
    if (filters?.dataFim) params.append('dataFim', filters.dataFim);
    if (filters?.clienteNome) params.append('clienteNome', filters.clienteNome);
    
    const query = params.toString();
    return this.request<any[]>(`/Pedido${query ? `?${query}` : ''}`);
  }

  async getPedidoById(id: number) {
    return this.request<any>(`/Pedido/${id}`);
  }

  async criarPedido(data: { cnpj: string }) {
    return this.request<any>('/Pedido', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async adicionarItem(codPedido: number, data: { codProduto: number; quantidade: number }) {
    return this.request<any>(`/Pedido/${codPedido}/itens`, {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async atualizarItem(codPedido: number, codProduto: number, data: { quantidade: number }) {
    return this.request<any>(`/Pedido/${codPedido}/itens/${codProduto}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async removerItem(codPedido: number, codProduto: number) {
    return this.request<any>(`/Pedido/${codPedido}/itens/${codProduto}`, {
      method: 'DELETE',
    });
  }
}

export const api = new ApiClient(API_BASE_URL);