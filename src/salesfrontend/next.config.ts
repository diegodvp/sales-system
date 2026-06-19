/** @type {import('next').NextConfig} */
const nextConfig = {
  // Ignorar erros de certificado SSL em desenvolvimento
  async rewrites() {
    return [];
  },
};

module.exports = nextConfig;