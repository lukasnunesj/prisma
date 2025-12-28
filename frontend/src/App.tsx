import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { useEffect, useState } from "react";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Categorias from "./pages/Categorias";
import FormasPagamento from "./pages/FormasPagamento";
import NotFound from "./pages/NotFound";
import { authService } from "@/services/auth/authService";
import { useAuthStore } from "@/stores/appStore";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30000,
      refetchOnWindowFocus: false,
    },
  },
});

const App = () => {
  const [bootstrapped, setBootstrapped] = useState(false);
  const { token, setToken, setUser } = useAuthStore();

  useEffect(() => {
    (async () => {
      try {
        await fetch(`${import.meta.env.VITE_API_URL ?? 'https://localhost:5001/api'}/auth/csrf`, {
          credentials: 'include',
        });

        const result = await authService.refresh();
        setToken(result.token);
        setUser(result.user);
      } catch (_err) {
        setToken(null);
        setUser(null);
      } finally {
        setBootstrapped(true);
      }
    })();
  }, [setToken, setUser]);

  if (!bootstrapped) {
    return null;
  }

  return (
    <QueryClientProvider client={queryClient}>
      <TooltipProvider>
        <Toaster />
        <Sonner />
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Navigate to={token ? "/dashboard" : "/login"} replace />} />
            <Route path="/login" element={<Login />} />
            <Route path="/dashboard" element={token ? <Dashboard /> : <Navigate to="/login" replace />} />
            <Route path="/categorias" element={token ? <Categorias /> : <Navigate to="/login" replace />} />
            <Route path="/formas-pagamento" element={token ? <FormasPagamento /> : <Navigate to="/login" replace />} />
            <Route path="*" element={<NotFound />} />
          </Routes>
        </BrowserRouter>
      </TooltipProvider>
    </QueryClientProvider>
  );
};

export default App;
