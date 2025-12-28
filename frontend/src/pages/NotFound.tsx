import { useLocation, Link } from "react-router-dom";
import { useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Home } from "lucide-react";

const NotFound = () => {
  const location = useLocation();

  useEffect(() => {
    console.error("404 Error:", location.pathname);
  }, [location.pathname]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-background">
      <div className="text-center">
        <div className="text-7xl font-bold text-primary mb-4">404</div>
        <h1 className="text-xl font-semibold mb-2">Página não encontrada</h1>
        <p className="text-muted-foreground mb-6">A página que você procura não existe.</p>
        <Link to="/dashboard">
          <Button className="gap-2">
            <Home className="w-4 h-4" />
            Ir para Dashboard
          </Button>
        </Link>
      </div>
    </div>
  );
};

export default NotFound;
