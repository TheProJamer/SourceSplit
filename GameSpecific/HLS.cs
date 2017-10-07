using System;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace LiveSplit.SourceSplit.GameSpecific
{
    class HLS : GameSupport
    {
        // how to match with demos:
        // start: map spawn
        // ending: the first tick after you kill nihilanth)

        private bool _onceFlag;
        private IntPtr _endDetectEntity;
        private IntPtr _startDetectEntity;

        public HLS()
        {
            this.GameTimingMethod = GameTimingMethod.EngineTicksWithPauses;
            this.FirstMap = "c1a0";
            this.LastMap = "c4a3";
        }

        public override void OnSessionStart(GameState state)
        {
            base.OnSessionStart(state);

            _onceFlag = false;
            _endDetectEntity = IntPtr.Zero;

            if (this.IsLastMap)
                _endDetectEntity = state.GetEntityByName("n_end_relay");
        }

        public override GameSupportResult OnUpdate(GameState state)
        {
            if (_onceFlag)
                
            if (this.IsFirstMap)
            {
                //Timer starts as soon as the player spawns in Half-Life Source. No need for any entity detection.
                Debug.WriteLine("hls start");
                _onceFlag = true;
                return GameSupportResult.PlayerGainedControl;
            }
            else if (this.IsLastMap && _endDetectEntity != IntPtr.Zero)
            {
                // "OnTrigger" "razortrain3,StartForward,,0,-1"
                // "OnTrigger" "outro_train_1,SetParent,razortrain3,0,-1"

                Vector3f trainPos;
                if (!state.GameProcess.ReadValue(_endDetectEntity + state.GameOffsets.BaseEntityAbsOriginOffset, out trainPos))
                    return GameSupportResult.DoNothing;

                // if the train started moving, stop timing
                if (!trainPos.BitEquals(_trainStartPos))
                {
                    Debug.WriteLine("hls end");
                    _onceFlag = true;
                    _endDetectEntity = IntPtr.Zero;
                    return GameSupportResult.PlayerLostControl;
                }
            }

            return GameSupportResult.DoNothing;
        }
    }
}
