using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform pivot;
    public Transform bulletSpawnPoint;
    public float force = 100.0f;
    public float reloadTime = 1.0f;
    float elevation = 0.0f;
	private NetworkStartPosition[] spawnPoints;
    Rigidbody bullet;
    bool loaded = true;

    public override void OnStartLocalPlayer()
    {
        CmdSpawn();
    }

    void Update()
    {
		if (!isLocalPlayer)
		{
			return;
		}
        var vertical = Input.GetAxis("Vertical") * Time.deltaTime * 150.0f;
        elevation = Mathf.Clamp(elevation - vertical, -80.0f, 0.0f);
        pivot.localRotation = Quaternion.Euler(elevation, 0.0f, 0.0f);
        if (loaded && Input.GetButton("Fire1"))
        {
            StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        CmdFire();
        loaded = false;
        yield return new WaitForSeconds(reloadTime);
        CmdSpawn();
        loaded = true;
    }

    [Command]
    void CmdSpawn()
    {
        // Create the Bullet from the Bullet Prefab
        var bulletObject = (GameObject)Instantiate (
            bulletPrefab,
            transform.position,
            transform.rotation);
        bullet = bulletObject.GetComponent<Rigidbody>();
        NetworkServer.Spawn(bulletObject);
    }
    
    [Command]
    void CmdFire()
    {
        if (bullet != null)
        {
            bullet.useGravity = true;
            bullet.AddForce(force * pivot.transform.forward, ForceMode.Impulse);
            bullet = null;
        }
    }    
}
