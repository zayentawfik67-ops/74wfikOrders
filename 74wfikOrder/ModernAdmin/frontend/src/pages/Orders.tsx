import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../api/axios';
import { useState } from 'react';
import { 
  Search, 
  Filter, 
  ChevronLeft, 
  ChevronRight,
  Eye,
  CheckCircle,
  XCircle,
  Package,
  CreditCard
} from 'lucide-react';

const Orders = () => {
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState('');
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['orders', page, status],
    queryFn: async () => {
      const res = await api.get('/orders', { params: { page, status } });
      return res.data;
    }
  });

  const updateStatusMutation = useMutation({
    mutationFn: async ({ id, newStatus }: { id: string, newStatus: string }) => {
      return api.patch(`/orders/${id}/status`, { status: newStatus });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    }
  });

  const getStatusColor = (s: string) => {
    switch (s) {
      case 'Paid': return 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400';
      case 'Pending': return 'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400';
      case 'Shipped': return 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400';
      case 'Cancelled': return 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400';
      default: return 'bg-slate-100 text-slate-700';
    }
  };

  if (isLoading) return <div className="p-8 text-center">Chargement des commandes...</div>;

  return (
    <div className="space-y-6">
      {/* Filters */}
      <div className="glass-card p-4 flex flex-wrap gap-4 items-center justify-between">
        <div className="relative flex-1 min-w-[200px]">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
          <input 
            type="text" 
            placeholder="Rechercher par N° de commande..." 
            className="input-field pl-10"
          />
        </div>
        
        <div className="flex gap-4">
          <select 
            value={status} 
            onChange={(e) => setStatus(e.target.value)}
            className="input-field"
          >
            <option value="">Tous les statuts</option>
            <option value="Pending">En attente</option>
            <option value="Paid">Payée</option>
            <option value="Shipped">Expédiée</option>
            <option value="Cancelled">Annulée</option>
          </select>
          <button className="flex items-center gap-2 px-4 py-2 bg-slate-100 dark:bg-slate-800 rounded-xl font-bold">
            <Filter size={18} />
            Plus de filtres
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="glass-card overflow-hidden">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="border-b border-slate-200 dark:border-slate-800 bg-slate-50/50 dark:bg-slate-800/50">
              <th className="p-4 font-bold text-slate-500 uppercase text-xs tracking-wider">Commande</th>
              <th className="p-4 font-bold text-slate-500 uppercase text-xs tracking-wider">Client</th>
              <th className="p-4 font-bold text-slate-500 uppercase text-xs tracking-wider">Total</th>
              <th className="p-4 font-bold text-slate-500 uppercase text-xs tracking-wider">Statut</th>
              <th className="p-4 font-bold text-slate-500 uppercase text-xs tracking-wider">Date</th>
              <th className="p-4 font-bold text-slate-500 uppercase text-xs tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody>
            {data?.orders.map((order: any) => (
              <tr key={order.id} className="border-b border-slate-100 dark:border-slate-800 hover:bg-slate-50/50 dark:hover:bg-slate-800/50 transition-colors">
                <td className="p-4 font-bold">#{order.orderNumber}</td>
                <td className="p-4">
                  <p className="font-bold">{order.Customer?.name}</p>
                  <p className="text-xs text-slate-500">{order.Customer?.email}</p>
                </td>
                <td className="p-4 font-black text-primary">{parseFloat(order.total).toLocaleString()} DT</td>
                <td className="p-4">
                  <span className={`px-3 py-1 rounded-full text-xs font-bold ${getStatusColor(order.status)}`}>
                    {order.status}
                  </span>
                </td>
                <td className="p-4 text-sm text-slate-500">
                  {new Date(order.createdAt).toLocaleDateString()}
                </td>
                <td className="p-4">
                  <div className="flex gap-2">
                    <button className="p-2 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-lg text-slate-500" title="Détails">
                      <Eye size={18} />
                    </button>
                    {order.status === 'Pending' && (
                      <button 
                        onClick={() => updateStatusMutation.mutate({ id: order.id, newStatus: 'Paid' })}
                        className="p-2 hover:bg-green-100 dark:hover:bg-green-900/30 rounded-lg text-green-500" 
                        title="Marquer comme payée"
                      >
                        <CheckCircle size={18} />
                      </button>
                    )}
                    {order.status === 'Paid' && (
                      <button 
                        onClick={() => updateStatusMutation.mutate({ id: order.id, newStatus: 'Shipped' })}
                        className="p-2 hover:bg-blue-100 dark:hover:bg-blue-900/30 rounded-lg text-blue-500" 
                        title="Expédier"
                      >
                        <Package size={18} />
                      </button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {/* Pagination */}
        <div className="p-4 flex items-center justify-between bg-slate-50/30 dark:bg-slate-800/30">
          <p className="text-sm text-slate-500">
            Affichage de <b>{(page - 1) * 10 + 1}</b> à <b>{Math.min(page * 10, data?.totalOrders)}</b> sur <b>{data?.totalOrders}</b> commandes
          </p>
          <div className="flex gap-2">
            <button 
              disabled={page === 1}
              onClick={() => setPage(p => p - 1)}
              className="p-2 border border-slate-200 dark:border-slate-700 rounded-lg disabled:opacity-50"
            >
              <ChevronLeft size={20} />
            </button>
            <button 
              disabled={page >= data?.totalPages}
              onClick={() => setPage(p => p + 1)}
              className="p-2 border border-slate-200 dark:border-slate-700 rounded-lg disabled:opacity-50"
            >
              <ChevronRight size={20} />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Orders;
