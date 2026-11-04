import { useState } from 'react';
import { useAuthStore } from '../store/useAuthStore';
import api from '../api/axios';
import { Lock, Mail, ShieldCheck, ArrowRight } from 'lucide-react';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [twoFactorCode, setTwoFactorCode] = useState('');
  const [require2FA, setRequire2FA] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const setAuth = useAuthStore(state => state.setAuth);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const res = await api.post('/auth/login', { email, password, twoFactorCode });
      
      if (res.data.require2FA) {
        setRequire2FA(true);
        setLoading(false);
        return;
      }

      setAuth(res.data.user, res.data.token);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Une erreur est survenue');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-950 flex items-center justify-center p-6 bg-[url('https://images.unsplash.com/photo-1555396273-367ea4eb4db5?q=80&w=2000&auto=format&fit=crop')] bg-cover bg-center relative">
      <div className="absolute inset-0 bg-slate-950/80 backdrop-blur-sm"></div>
      
      <div className="glass-card w-full max-w-md p-8 relative z-10 border-white/10">
        <div className="text-center mb-10">
          <div className="w-16 h-16 bg-primary rounded-2xl flex items-center justify-center text-white font-black text-3xl mx-auto mb-4 shadow-2xl shadow-primary/40">
            74
          </div>
          <h2 className="text-2xl font-black tracking-tight text-white">KayuSushi Admin</h2>
          <p className="text-slate-400 mt-2">Bienvenue sur votre portail sécurisé</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          {error && (
            <div className="bg-red-500/10 border border-red-500/20 text-red-500 p-4 rounded-xl text-sm font-bold text-center">
              {error}
            </div>
          )}

          {!require2FA ? (
            <>
              <div className="space-y-2">
                <label className="text-xs font-black uppercase text-slate-500 ml-1">Email professionnel</label>
                <div className="relative">
                  <Mail className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                  <input 
                    type="email" 
                    required 
                    className="input-field pl-12 bg-white/5 border-white/10 text-white focus:bg-white/10" 
                    placeholder="admin@74wfikorder.com"
                    value={email}
                    onChange={e => setEmail(e.target.value)}
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-xs font-black uppercase text-slate-500 ml-1">Mot de passe</label>
                <div className="relative">
                  <Lock className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                  <input 
                    type="password" 
                    required 
                    className="input-field pl-12 bg-white/5 border-white/10 text-white focus:bg-white/10" 
                    placeholder="••••••••"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                  />
                </div>
              </div>
            </>
          ) : (
            <div className="space-y-2">
              <label className="text-xs font-black uppercase text-slate-500 ml-1">Code de vérification (2FA)</label>
              <div className="relative">
                <ShieldCheck className="absolute left-4 top-1/2 -translate-y-1/2 text-primary" size={18} />
                <input 
                  type="text" 
                  required 
                  className="input-field pl-12 bg-white/5 border-white/10 text-white text-center tracking-[1em] font-black text-xl" 
                  placeholder="000000"
                  maxLength={6}
                  value={twoFactorCode}
                  onChange={e => setTwoFactorCode(e.target.value)}
                />
              </div>
              <p className="text-[10px] text-slate-500 text-center mt-2 italic">Entrez le code de votre application d'authentification</p>
            </div>
          )}

          <button 
            type="submit" 
            disabled={loading}
            className="w-full bg-primary hover:bg-primary-dark text-white font-black py-4 rounded-xl transition-all flex items-center justify-center gap-2 group disabled:opacity-50"
          >
            {loading ? 'Connexion en cours...' : (
              <>
                {require2FA ? 'Vérifier' : 'Se connecter'}
                <ArrowRight size={18} className="group-hover:translate-x-1 transition-transform" />
              </>
            )}
          </button>
        </form>

        <p className="text-center text-xs text-slate-500 mt-8">
          Système sécurisé KayuSushi. Accès réservé au personnel autorisé uniquement.
        </p>
      </div>
    </div>
  );
};

export default Login;
