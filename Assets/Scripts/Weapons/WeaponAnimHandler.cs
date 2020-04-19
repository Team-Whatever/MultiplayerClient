using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimHandler : MonoBehaviour
{
	UnitBase owner;

    void Awake()
	{
		owner = GetComponentInParent<UnitBase>();
	}

    public void BeginAttack()
	{
        //DebugExtension.LogLevel( owner.ToString() + " : BeginAttack", DebugExtension.LogType.Gameplay );
        //owner.OnBeginAttack();
	}

    public void Fire()
    {
        //DebugExtension.LogLevel( owner.ToString() + " : Hit", DebugExtension.LogType.Gameplay );
        //owner.OnFire();
    }

    public void EndAttack()
	{
        //DebugExtension.LogLevel( owner.ToString() + " : EndAttack", DebugExtension.LogType.Gameplay );
        owner.OnEndAttack();
	}


}
