using System;
using System.Collections.Generic;

namespace MCAutoVote.Utilities.Multithreading
{
    public class ResourceLock
    {
        private class Handle : IDisposable
        {
            private ResourceLock @lock;
            public Handle(ResourceLock @lock) {
                this.@lock = @lock;
                @lock.AddHandle(this);
            }

            void IDisposable.Dispose() => @lock.RemoveHandle(this);
        }

        private readonly object syncRoot = new object();

        private HashSet<Handle> handles = new HashSet<Handle>();

        public event Action Locked;
        public event Action Released;

        public bool IsLocked => handles.Count > 0;

        public ResourceLock() { }
        public ResourceLock(Action locked, Action released)
        {
            Locked += locked;
            Released += released;
        }

        public IDisposable Use() => new Handle(this);

        private void AddHandle(Handle handle)
        {
            if (handle == null) throw new ArgumentNullException("Resource handle is null!");

            lock (syncRoot)
            {
                bool noHandles = handles.Count == 0;
                handles.Add(handle);
                if (noHandles) Locked?.Invoke(); //if no handles was present before, raise LOCKED
            }
        }

        private void RemoveHandle(Handle handle)
        {
            if (handle == null) throw new ArgumentNullException("Resource handle is null!");
            if (!handles.Contains(handle)) throw new ArgumentException("Given resource handle not associated with this resource!");

            lock (syncRoot)
            {
                handles.Remove(handle);
                if (handles.Count == 0) Released?.Invoke();//if no handles left, raise RELEASED
            }
        }
    }
}
