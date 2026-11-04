import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../api/axios';
import { useState } from 'react';
import { 
  UserCircle, 
  Mail, 
  Phone, 
  ShieldAlert,
  Send,
  CheckCircle2,
  AlertCircle
} from 'lucide-react';

const Customers = () => {
  const queryClient = useQueryClient();
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ email: '', phone: '' });
  const [otp, setOtp] = useState('');
  const [showOtpInput, setShowOtpInput] = useState(false);

  const { data: customers, isLoading } = useQuery({
    queryKey: ['customers'],
    queryFn: async () => {
      const res = await api.get('/customers');
      return res.data;
    }
  });

  const requestChangeMutation = useMutation({
    mutationFn: async ({ id, data }: any) => {
      return api.post(`/customers/${id}/request-change`, data);
    },
    onSuccess: () => {
      setShowOtpInput(true);
      queryClient.invalidateQueries({ queryKey: ['customers'] });
    }
  });

  const verifyChangeMutation = useMutation({
    mutationFn: async ({ id, otp }: any) => {
      return api.post(`/customers/${id}/verify-change`, { otp });
    },
    onSuccess: () => {
      setShowOtpInput(false);
      setEditingId(null);
      setOtp('');
      queryClient.invalidateQueries({ queryKey: ['customers'] });
      alert('Informations mises à jour avec succès !');
    }
  });

  if (isLoading) return <div className="p-8 text-center">Chargement des clients...</div>;

  return (
    <div className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
        {customers?.map((customer: any) => (
          <div key={customer.id} className="glass-card p-6 flex flex-col gap-4">
            <div className="flex items-center gap-4">
              <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center text-primary">
                <UserCircle size={48} />
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold">{customer.name}</h3>
                <span className={`text-[10px] uppercase font-black px-2 py-0.5 rounded-full ${
                  customer.status === 'Verified' ? 'bg-green-100 text-green-700' : 'bg-orange-100 text-orange-700'
                }`}>
                  {customer.status}
                </span>
              </div>
            </div>

            <div className="space-y-3 pt-2">
              <div className="flex items-center gap-3 text-sm text-slate-600 dark:text-slate-400">
                <Mail size={16} />
                <span>{customer.email}</span>
              </div>
              <div className="flex items-center gap-3 text-sm text-slate-600 dark:text-slate-400">
                <Phone size={16} />
                <span>{customer.phone}</span>
              </div>
            </div>

            {editingId === customer.id ? (
              <div className="pt-4 mt-4 border-t border-slate-100 dark:border-slate-800 space-y-4">
                {!showOtpInput ? (
                  <>
                    <p className="text-xs font-bold text-orange-500 flex items-center gap-1">
                      <ShieldAlert size={14} /> Demander une modification
                    </p>
                    <input 
                      type="email" 
                      placeholder="Nouvel Email" 
                      className="input-field text-sm"
                      value={formData.email}
                      onChange={e => setFormData({...formData, email: e.target.value})}
                    />
                    <input 
                      type="text" 
                      placeholder="Nouveau Téléphone" 
                      className="input-field text-sm"
                      value={formData.phone}
                      onChange={e => setFormData({...formData, phone: e.target.value})}
                    />
                    <div className="flex gap-2">
                      <button 
                        onClick={() => requestChangeMutation.mutate({ id: customer.id, data: formData })}
                        className="btn-primary flex-1 py-1.5 text-sm flex items-center justify-center gap-2"
                      >
                        <Send size={14} /> Envoyer OTP
                      </button>
                      <button onClick={() => setEditingId(null)} className="flex-1 py-1.5 text-sm font-bold">Annuler</button>
                    </div>
                  </>
                ) : (
                  <>
                    <p className="text-xs font-bold text-green-500 flex items-center gap-1">
                      <CheckCircle2 size={14} /> Entrer le code reçu par le client
                    </p>
                    <input 
                      type="text" 
                      placeholder="Code OTP (6 chiffres)" 
                      className="input-field text-sm text-center tracking-widest font-black"
                      value={otp}
                      onChange={e => setOtp(e.target.value)}
                    />
                    <button 
                      onClick={() => verifyChangeMutation.mutate({ id: customer.id, otp })}
                      className="btn-primary w-full py-1.5 text-sm"
                    >
                      Vérifier et Appliquer
                    </button>
                  </>
                )}
              </div>
            ) : (
              <button 
                onClick={() => {
                  setEditingId(customer.id);
                  setFormData({ email: customer.email, phone: customer.phone });
                  setShowOtpInput(false);
                }}
                className="mt-4 py-2 border border-slate-200 dark:border-slate-700 rounded-xl text-sm font-bold hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors"
              >
                Gérer les accès
              </button>
            )}

            {customer.status === 'Pending_Verification' && !editingId && (
              <div className="mt-2 p-2 bg-orange-50 dark:bg-orange-900/20 rounded-lg flex items-center gap-2 text-[10px] text-orange-600 dark:text-orange-400 font-bold">
                <AlertCircle size={12} /> Modification en attente de validation
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default Customers;
