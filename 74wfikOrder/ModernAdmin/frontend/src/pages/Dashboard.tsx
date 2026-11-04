import { useQuery } from '@tanstack/react-query';
import api from '../api/axios';
import { 
  TrendingUp, 
  ShoppingBag, 
  Clock, 
  Users,
  ArrowUpRight,
  ArrowDownRight
} from 'lucide-react';
import { 
  AreaChart, 
  Area, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer 
} from 'recharts';

const data = [
  { name: 'Lun', sales: 4000 },
  { name: 'Mar', sales: 3000 },
  { name: 'Mer', sales: 2000 },
  { name: 'Jeu', sales: 2780 },
  { name: 'Ven', sales: 1890 },
  { name: 'Sam', sales: 2390 },
  { name: 'Dim', sales: 3490 },
];

const StatCard = ({ title, value, icon: Icon, color, trend }: any) => (
  <div className="glass-card p-6 flex flex-col gap-4">
    <div className="flex justify-between items-start">
      <div className={`p-3 rounded-xl ${color} bg-opacity-10 text-${color.split('-')[1]}`}>
        <Icon size={24} />
      </div>
      {trend && (
        <span className={`flex items-center gap-1 text-sm font-bold ${trend > 0 ? 'text-green-500' : 'text-red-500'}`}>
          {trend > 0 ? <ArrowUpRight size={16} /> : <ArrowDownRight size={16} />}
          {Math.abs(trend)}%
        </span>
      )}
    </div>
    <div>
      <p className="text-slate-500 dark:text-slate-400 font-medium">{title}</p>
      <h3 className="text-2xl font-black mt-1">{value}</h3>
    </div>
  </div>
);

const Dashboard = () => {
  const { data: stats, isLoading } = useQuery({
    queryKey: ['stats'],
    queryFn: async () => {
      const res = await api.get('/orders/stats');
      return res.data;
    }
  });

  if (isLoading) return <div className="p-8 text-center">Chargement des données...</div>;

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard 
          title="Total Commandes" 
          value={stats?.totalOrders || 0} 
          icon={ShoppingBag} 
          color="bg-blue-500" 
          trend={12} 
        />
        <StatCard 
          title="Revenus Total" 
          value={`${(stats?.revenue || 0).toLocaleString()} DT`} 
          icon={TrendingUp} 
          color="bg-green-500" 
          trend={8} 
        />
        <StatCard 
          title="En attente" 
          value={stats?.pendingOrders || 0} 
          icon={Clock} 
          color="bg-orange-500" 
        />
        <StatCard 
          title="Nouveaux Clients" 
          value="24" 
          icon={Users} 
          color="bg-purple-500" 
          trend={-5} 
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 glass-card p-6">
          <h3 className="text-lg font-bold mb-6">Tendances de Ventes</h3>
          <div className="h-80 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={data}>
                <defs>
                  <linearGradient id="colorSales" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#e85d04" stopOpacity={0.3}/>
                    <stop offset="95%" stopColor="#e85d04" stopOpacity={0}/>
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#e2e8f0" />
                <XAxis dataKey="name" axisLine={false} tickLine={false} />
                <YAxis axisLine={false} tickLine={false} />
                <Tooltip 
                  contentStyle={{ 
                    backgroundColor: 'rgba(255, 255, 255, 0.8)', 
                    borderRadius: '12px', 
                    border: 'none',
                    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)'
                  }} 
                />
                <Area type="monotone" dataKey="sales" stroke="#e85d04" strokeWidth={3} fillOpacity={1} fill="url(#colorSales)" />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="glass-card p-6">
          <h3 className="text-lg font-bold mb-6">Activité Récente</h3>
          <div className="space-y-6">
            {stats?.recentActivity?.map((log: any) => (
              <div key={log.id} className="flex gap-4">
                <div className="w-10 h-10 rounded-full bg-slate-100 dark:bg-slate-800 flex items-center justify-center text-primary font-bold">
                  {log.adminName?.[0]}
                </div>
                <div className="flex-1">
                  <p className="text-sm font-bold">{log.action}</p>
                  <p className="text-xs text-slate-500">{log.details}</p>
                  <p className="text-[10px] text-slate-400 mt-1">
                    {new Date(log.createdAt).toLocaleTimeString()}
                  </p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
