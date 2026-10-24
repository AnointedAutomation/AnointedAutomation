import { fileURLToPath } from 'node:url';

// The example links the SDK via "file:../..", so Turbopack must treat the SDK package as the project root to resolve it.
const nextConfig = {
  turbopack: {
    root: fileURLToPath(new URL('../..', import.meta.url)),
  },
};

export default nextConfig;
