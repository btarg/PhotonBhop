using UnityEngine;

namespace Utility {
	/// <summary>
	/// The PooledObject is a base class for recyclable game objects. The two most important things to remember when working with pooled objects are:
	/// 
	/// 1) Do not call Destroy on a pooled object - always use ObjectPool.Recycle()
	/// 2) A recycled object is *not* a new object - it may have changed during its last use and may no longer be the same as the original prefab
	/// 
	/// The last point is especially important, and though it can sometimes be ignored (a bullet prefab that has no state other than position and orientation
	/// will work just fine since both of these will be reset automatically), you mostly do need to explicitly decide what to reset when
	/// the object is recycled. 
	/// 
	/// To reset the recycled instance you may override the OnRecycled method - it is recommended to use this instead of Start() for initializing 
	/// pooled objects (it is also called on first instantation) and only use Awake to setup things that will not change over the life of the object.
	/// </summary>
	public class PooledObject : MonoBehaviour {
		public ObjectPool pool { get; set; }

		public virtual void OnRecycled() {
		}
	}
}