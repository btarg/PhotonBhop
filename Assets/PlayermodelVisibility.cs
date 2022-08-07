using UnityEngine;
using Fusion;

public class PlayermodelVisibility : NetworkBehaviour
{
    public override void Spawned()
    {
        if (Object.HasInputAuthority) {
            // Local player invis
            gameObject.layer = 3;
        }
    }

}
