using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuideCube
{
    public abstract class GCubeState
    {
        protected GCubeController controller;

        public void Start(GCubeController controller)
        {
            this.controller = controller;

            Start();
        }
        protected virtual void Start() { }

        public virtual void End() { }

        public virtual void Update() { }
    }
}