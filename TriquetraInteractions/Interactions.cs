using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Triquetra
{
    public static class Interactions
    {
        const float knobTwistSpeed = 0.025f;
        public static void TwistKnob(VRTwistKnob twistKnob, bool antiClockwise, float delta = knobTwistSpeed)
        {
            //twistKnob.OnTwistDelta?.Invoke(antiClockwise ? -0.1f : 0.1f);
            twistKnob.SetKnobValue(twistKnob.currentValue + (antiClockwise ? -knobTwistSpeed : knobTwistSpeed));
        }

        public static void MoveLever(VRLever lever, int delta = 1, bool clamp = false)
        {
            int nextState;
            if (clamp)
                nextState = Math.Max(Math.Min(lever.currentState + delta, lever.states - 1), 0);
            else
                nextState = (lever.currentState + delta) % lever.states;
            lever.RemoteSetState(nextState);
        }

        public static void MoveTwistKnobInt(VRTwistKnobInt twistKnobInt, int delta = 1, bool clamp = false)
        {
            int nextState;
            if (clamp)
                nextState = Math.Max(Math.Min(twistKnobInt.currentState + delta, twistKnobInt.states - 1), 0);
            else
                nextState = (twistKnobInt.currentState + delta) % twistKnobInt.states;
            twistKnobInt.RemoteSetState(nextState);
        }

        public static void MoveThrottle(VRThrottle throttle, float delta)
        {
            throttle.RemoteSetThrottleForceEvents(Math.Min(Math.Max(throttle.currentThrottle + delta, 0f), 1f));
        }
        public static void SetThrottle(VRThrottle throttle, float value)
        {
            throttle.RemoteSetThrottleForceEvents(Math.Min(Math.Max(value, 0f), 1f));
        }

        public static void Interact(VRInteractable interactable)
        {
            VRLever lever = interactable.GetComponent<VRLever>();
            VRTwistKnobInt twistKnobInt = interactable.GetComponent<VRTwistKnobInt>();
            VRButton button = interactable.GetComponent<VRButton>();
            EjectHandle eject = interactable.GetComponent<EjectHandle>();
            VRDoor door = interactable.GetComponent<VRDoor>();
            VRKeyboard.VRKey vrKey = interactable.GetComponent<VRKeyboard.VRKey>();

            if (lever != null)
            {
                MoveLever(lever, 1);
            }
            else if (twistKnobInt != null)
            {
                MoveTwistKnobInt(twistKnobInt, 1);
            }
            else if (vrKey != null)
            {
                vrKey.OnPress();
            }
            else if (button != null)
            {
                interactable.OnInteract?.Invoke();
            }
            else if (eject != null)
            {
                eject.OnHandlePull?.Invoke();
            }
            else if (door != null)
            {
                door.RemoteSetState(door.isLatched ? 1f : 0f); // 1f = to open, 0f = to close
            }
            else
            {
                try
                {
                    interactable.StartInteraction();
                }
                catch (NullReferenceException)
                {
                    interactable.OnInteract?.Invoke();
                }
            }
        }

        public static void AntiInteract(VRInteractable interactable)
        {
            VRButton button = interactable.GetComponent<VRButton>();
            if (button != null)
            {
                interactable.OnStopInteract?.Invoke();
            }
            else
            {
                interactable.StopInteraction();
            }
        }
    }
}
