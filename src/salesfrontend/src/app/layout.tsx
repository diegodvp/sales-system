import type { Metadata } from 'next';
import Header from '@/components/Layout/Header';
import './globals.css';

export const metadata: Metadata = {
  title: 'Sales System',
  description: 'Sistema de Vendas',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="pt-BR">
      <body className="bg-gray-100 font-sans">
        <Header />
        <main className="max-w-7xl mx-auto px-4 py-8">
          {children}
        </main>
      </body>
    </html>
  );
}