using Newtonsoft.Json;

namespace ReeCamera {
    public struct LayerFilter {
        public bool Layer0;
        public bool Layer1;
        public bool Layer2;
        public bool Layer3;
        public bool Layer4;
        public bool Layer5;
        public bool Layer6;
        public bool Layer7;
        public bool Layer8;
        public bool Layer9;
        public bool Layer10;
        public bool Layer11;
        public bool Layer12;
        public bool Layer13;
        public bool Layer14;
        public bool Layer15;
        public bool Layer16;
        public bool Layer17;
        public bool Layer18;
        public bool Layer19;
        public bool Layer20;
        public bool Layer21;
        public bool Layer22;
        public bool Layer23;
        public bool Layer24;
        public bool Layer25;
        public bool Layer26;
        public bool Layer27;
        public bool Layer28;
        public bool Layer29;
        public bool Layer30;
        public bool Layer31;

        [JsonConstructor]
        public LayerFilter(int _) {
            Layer0 = true;
            Layer1 = true;
            Layer2 = true;
            Layer3 = false;
            Layer4 = true;
            Layer5 = true;
            Layer6 = false;
            Layer7 = false;
            Layer8 = true;
            Layer9 = true;
            Layer10 = true;
            Layer11 = true;
            Layer12 = true;
            Layer13 = true;
            Layer14 = true;
            Layer15 = true;
            Layer16 = true;
            Layer17 = true;
            Layer18 = false;
            Layer19 = true;
            Layer20 = true;
            Layer21 = false;
            Layer22 = false;
            Layer23 = false;
            Layer24 = false;
            Layer25 = false;
            Layer26 = false;
            Layer27 = true;
            Layer28 = true;
            Layer29 = true;
            Layer30 = false;
            Layer31 = false;
        }

        public static LayerFilter Default => new LayerFilter(0);

        public static LayerFilter FromCullingMask(int mask) {
            var filter = new LayerFilter {
                Layer0 = (mask & (1 << 0)) != 0,
                Layer1 = (mask & (1 << 1)) != 0,
                Layer2 = (mask & (1 << 2)) != 0,
                Layer3 = (mask & (1 << 3)) != 0,
                Layer4 = (mask & (1 << 4)) != 0,
                Layer5 = (mask & (1 << 5)) != 0,
                Layer6 = (mask & (1 << 6)) != 0,
                Layer7 = (mask & (1 << 7)) != 0,
                Layer8 = (mask & (1 << 8)) != 0,
                Layer9 = (mask & (1 << 9)) != 0,
                Layer10 = (mask & (1 << 10)) != 0,
                Layer11 = (mask & (1 << 11)) != 0,
                Layer12 = (mask & (1 << 12)) != 0,
                Layer13 = (mask & (1 << 13)) != 0,
                Layer14 = (mask & (1 << 14)) != 0,
                Layer15 = (mask & (1 << 15)) != 0,
                Layer16 = (mask & (1 << 16)) != 0,
                Layer17 = (mask & (1 << 17)) != 0,
                Layer18 = (mask & (1 << 18)) != 0,
                Layer19 = (mask & (1 << 19)) != 0,
                Layer20 = (mask & (1 << 20)) != 0,
                Layer21 = (mask & (1 << 21)) != 0,
                Layer22 = (mask & (1 << 22)) != 0,
                Layer23 = (mask & (1 << 23)) != 0,
                Layer24 = (mask & (1 << 24)) != 0,
                Layer25 = (mask & (1 << 25)) != 0,
                Layer26 = (mask & (1 << 26)) != 0,
                Layer27 = (mask & (1 << 27)) != 0,
                Layer28 = (mask & (1 << 28)) != 0,
                Layer29 = (mask & (1 << 29)) != 0,
                Layer30 = (mask & (1 << 30)) != 0,
                Layer31 = (mask & (1 << 31)) != 0
            };
            return filter;
        }

        [JsonIgnore]
        public int CullingMask {
            get {
                var cullingMask = 0;
                if (Layer0) cullingMask |= 1 << 0;
                if (Layer1) cullingMask |= 1 << 1;
                if (Layer2) cullingMask |= 1 << 2;
                if (Layer3) cullingMask |= 1 << 3;
                if (Layer4) cullingMask |= 1 << 4;
                if (Layer5) cullingMask |= 1 << 5;
                if (Layer6) cullingMask |= 1 << 6;
                if (Layer7) cullingMask |= 1 << 7;
                if (Layer8) cullingMask |= 1 << 8;
                if (Layer9) cullingMask |= 1 << 9;
                if (Layer10) cullingMask |= 1 << 10;
                if (Layer11) cullingMask |= 1 << 11;
                if (Layer12) cullingMask |= 1 << 12;
                if (Layer13) cullingMask |= 1 << 13;
                if (Layer14) cullingMask |= 1 << 14;
                if (Layer15) cullingMask |= 1 << 15;
                if (Layer16) cullingMask |= 1 << 16;
                if (Layer17) cullingMask |= 1 << 17;
                if (Layer18) cullingMask |= 1 << 18;
                if (Layer19) cullingMask |= 1 << 19;
                if (Layer20) cullingMask |= 1 << 20;
                if (Layer21) cullingMask |= 1 << 21;
                if (Layer22) cullingMask |= 1 << 22;
                if (Layer23) cullingMask |= 1 << 23;
                if (Layer24) cullingMask |= 1 << 24;
                if (Layer25) cullingMask |= 1 << 25;
                if (Layer26) cullingMask |= 1 << 26;
                if (Layer27) cullingMask |= 1 << 27;
                if (Layer28) cullingMask |= 1 << 28;
                if (Layer29) cullingMask |= 1 << 29;
                if (Layer30) cullingMask |= 1 << 30;
                if (Layer31) cullingMask |= 1 << 31;
                return cullingMask;
            }
        }
    }
}