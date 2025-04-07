// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("4oJS6+mf0q4Pm2G4at92kzu6ACkcid9rHE3lSk7JGz+/wT+f8AlCuiSQ4ME94lhPeNer5Rpc8rKWA+IXhxnLCHkuUP0xKkeLRAp+lRDDuJP/RUA5D42B/CV4zj8C9Kma4Qhmr2Dj7eLSYOPo4GDj4+JYK4ubKhWZ0mDjwNLv5OvIZKpkFe/j4+Pn4uE6vtmaaRI6cI3wDDAP179ExC0HUEUQiO7MSDP8/EsusEt4ZzJlF3Dm4b6OehCS7ke0eU2WpSZNwh3ZeFjQDIVK0MWguj8OOcOoXGNzHJMs1UDzvdgPpQH+CczgLkHzs/Kz6HFP+tFyc2iJ1DS/61fPy3SH9bo5bvWPDu3321rx9Ke0uyrrmS3n83wn3g0Hylyx0aTEM+Dh4+Lj");
        private static int[] order = new int[] { 11,12,5,6,4,12,11,7,13,10,13,11,12,13,14 };
        private static int key = 226;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
