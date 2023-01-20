//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/_HadiSplines/Examples/SplineMovementActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Hadi.Splines
{
    public partial class @SplineMovementActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @SplineMovementActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""SplineMovementActions"",
    ""maps"": [
        {
            ""name"": ""Splines"",
            ""id"": ""c77d529f-b689-43c6-8ecf-a4e2bfa54fff"",
            ""actions"": [
                {
                    ""name"": ""MoveForwards"",
                    ""type"": ""Button"",
                    ""id"": ""57e3c76f-6ac2-4a45-af25-80aadfecc3ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveBackwards"",
                    ""type"": ""Button"",
                    ""id"": ""f22abdfd-78c3-4d67-8cd6-784fd108f7be"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ea07ed27-7235-4ab8-8dab-c99584842ba2"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""acf993f8-8b9d-49ad-96a3-b09acf484ada"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd54239a-a57e-4778-9bd5-774830a5a404"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b2a40b5d-ec82-47cf-9a1d-4f9a6c516a3b"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""828e44c2-e8e9-4ea7-9771-b1be31a0ae0a"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBackwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""65be0480-cfc9-4d6f-b665-b9cea6c9a216"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBackwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed012540-71fe-4640-94d6-43d1cc4990be"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBackwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e58a4de-4534-4495-bc62-51e85030703a"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBackwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Splines
            m_Splines = asset.FindActionMap("Splines", throwIfNotFound: true);
            m_Splines_MoveForwards = m_Splines.FindAction("MoveForwards", throwIfNotFound: true);
            m_Splines_MoveBackwards = m_Splines.FindAction("MoveBackwards", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Splines
        private readonly InputActionMap m_Splines;
        private ISplinesActions m_SplinesActionsCallbackInterface;
        private readonly InputAction m_Splines_MoveForwards;
        private readonly InputAction m_Splines_MoveBackwards;
        public struct SplinesActions
        {
            private @SplineMovementActions m_Wrapper;
            public SplinesActions(@SplineMovementActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @MoveForwards => m_Wrapper.m_Splines_MoveForwards;
            public InputAction @MoveBackwards => m_Wrapper.m_Splines_MoveBackwards;
            public InputActionMap Get() { return m_Wrapper.m_Splines; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(SplinesActions set) { return set.Get(); }
            public void SetCallbacks(ISplinesActions instance)
            {
                if (m_Wrapper.m_SplinesActionsCallbackInterface != null)
                {
                    @MoveForwards.started -= m_Wrapper.m_SplinesActionsCallbackInterface.OnMoveForwards;
                    @MoveForwards.performed -= m_Wrapper.m_SplinesActionsCallbackInterface.OnMoveForwards;
                    @MoveForwards.canceled -= m_Wrapper.m_SplinesActionsCallbackInterface.OnMoveForwards;
                    @MoveBackwards.started -= m_Wrapper.m_SplinesActionsCallbackInterface.OnMoveBackwards;
                    @MoveBackwards.performed -= m_Wrapper.m_SplinesActionsCallbackInterface.OnMoveBackwards;
                    @MoveBackwards.canceled -= m_Wrapper.m_SplinesActionsCallbackInterface.OnMoveBackwards;
                }
                m_Wrapper.m_SplinesActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @MoveForwards.started += instance.OnMoveForwards;
                    @MoveForwards.performed += instance.OnMoveForwards;
                    @MoveForwards.canceled += instance.OnMoveForwards;
                    @MoveBackwards.started += instance.OnMoveBackwards;
                    @MoveBackwards.performed += instance.OnMoveBackwards;
                    @MoveBackwards.canceled += instance.OnMoveBackwards;
                }
            }
        }
        public SplinesActions @Splines => new SplinesActions(this);
        public interface ISplinesActions
        {
            void OnMoveForwards(InputAction.CallbackContext context);
            void OnMoveBackwards(InputAction.CallbackContext context);
        }
    }
}