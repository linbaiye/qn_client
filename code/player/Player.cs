
namespace QnClient.code.player;

public partial class Player : AbstractPlayer
{

    /*public override void _UnhandledInput(InputEvent @event)
    {
        TestPlay player = _animationPlayer.PlayBlade2HAttack;
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Keycode == Key.H)
            {
                if (_hatName == null)
                {
                    _hatName = "v16";
                    PutOnHat(_hatName);
                }
                else
                {
                    _hatName = null;
                    HideHat();
                }
            }
            else if (eventKey.Keycode == Key.L)
            {
                if (_legName == null)
                {
                    _legName = "r1";
                    PutOnLeg(_legName);
                }
                else
                {
                    _legName = null;
                    HideLeg();
                }
            } 
            else if (eventKey.Keycode == Key.B)
            {
                if (_bootName== null)
                {
                    _bootName = "q1";
                    PutOnBoot(_bootName);
                }
                else
                {
                    _bootName= null;
                    HideBoot();
                }
            }
            else if (eventKey.Keycode == Key.W)
            {
                if (_leftWrist == null)
                {
                    _leftWrist = "o1";
                    _rightWrist = "s1";
                    PutOnWrist(_leftWrist, _rightWrist);
                }
                else
                {
                    _leftWrist = null;
                    _rightWrist = null;
                    HideWrist();
                }
            }
            else if (eventKey.Keycode == Key.C)
            {
                if (_chest == null)
                {
                    _chest = "p1";
                    PutOnVest(_chest);
                }
                else
                {
                    HideVest();
                }
            }
            else if (eventKey.Keycode == Key.A)
            {
                if (_armor == null)
                {
                    _armor = "t1";
                    PutOnArmor(_armor);
                }
                else
                {
                    HideArmor();
                }
            }
            else if (eventKey.Keycode == Key.R)
            {
                if (_armor == null)
                {
                    _armor = "u1";
                    PutOnHair(_armor);
                }
                else
                {
                    HideHair();
                }
            }
            else if (eventKey.Keycode == Key.E)
            {
                if (_weapon == null)
                {
                    _weapon = "w1";
                    PutOnWeapon(_weapon);
                    _animationPlayer.SetBladeEffectAnimation("_232");
                }
                else
                {
                    HideWeapon();
                }
            }
            else if (eventKey.Keycode >= Key.Key1 && eventKey.Keycode <= Key.Key8)
            {
                player.Invoke((CreatureDirection)((int)eventKey.Keycode - (int)Key.Key1));
            }
        }
    }*/
}