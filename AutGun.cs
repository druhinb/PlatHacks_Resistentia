using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAutomatic : MonoBehaviour
{
    //Animator component
    Animator anim;

    [Header("Player Camera")]

    //Main gun camera
    public Camera playerCamera;

    [Header("Gun Camera Options")]

    public float fovSpeed = 15.0f;

    public float defaultFov = 40.0f;
    public float aimFov = 15.0f;

    [Header("UI Weapon Name")]
    public string weaponName;
    private string storedWeaponName;

    [Header("Weapon Sway")]

    public bool weaponSway;
    public float swayAmount = 0.02f;
	public float maxSwayAmount = 0.06f;
	public float swaySmoothValue = 4.0f;
    private Vector3 initialSwayPosition;
    private float lastFired;

    [Header("Weapon Settings")]
    public float fireRate;
    public bool autoReload;
    public float autoReloadDelay;
    private bool isReloading;
    private bool isRunning;
	//Check if aiming
	private bool isAiming;
	//Check if walking
	private bool isWalking;
	
    private int currentAmmo;
    public int ammo;
    private bool outOfAmmo;

    [Header("Bullet Settings")]
    public float bulletForce = 400.0f;

    [Header("Grenade Settings")]
    public float grenadeSpawnDelay = 0.35f;

    [Header("Muzzleflash Settings")]
    public bool randomMuzzleflash = false;
    private int minRandomValue = 1;

    [Range(2,25)]
    public int maxRandomValue = 5;
    private int randomMuzzleflashValue;
    public bool enableMuzzleflash = true;
    public ParticleSystem muzzleParticles;
    public bool enableSparks = true;
    public ParticleSystem sparkParticles;
    public int minSparkEmission = 1;
    public int maxSparkEmission = 7;

    [Header("Muzzleflash Light Settings")]
    public Light muzzleflashLight;
    public float lightDuration = 0.02f;

    [Header("Audio Source")]
    public AudioSource mainAudioSource;
    public AudioSource shootAudioSource;

    [System.Serializable]
    public class prefabs
    {
        [Header("Prefabs")]
        public Transform bulletPrefab;
        public Transform casingPrefab;
        public Transform grenadePrefab;
    }
    public prefabs Prefabs;

    [System.Serializable]
    public class spawnpoints
    {
        [Header("Prefabs")]
        public Transform casingSpawnPoint;
        public Transform bulletSpawnPoint;
        public Transform grenadeSpawnPoint;
    }
    public spawnpoints Spawnpoints;

    [System.Serializable]
	public class soundClips
	{
		public AudioClip shootSound;
		public AudioClip takeOutSound;
		public AudioClip holsterSound;
		public AudioClip reloadSoundOutOfAmmo;
		public AudioClip reloadSoundAmmoLeft;
		public AudioClip aimSound;
	}
	public soundClips SoundClips;
    private bool soundHasPlayed = false;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        currentAmmo = ammo;

        muzzleflashLight.enabled = false;
    }

    private void Start()
    {
        storedWeaponName = weaponName;

        initialSwayPosition = transform.localPosition;

        shootAudioSource.clip = SoundClips.shootSound;
    }

    private void LateUpdate() 
    {
        if (weaponSway == true)
        {
            float movementX = -Input.GetAxis("Horizontal") * swayAmount;
            float movementY = -Input.GetAxis ("Vertical") * swayAmount;

            movementX = Mathf.Clamp 
				(movementX, -maxSwayAmount, maxSwayAmount);
			movementY = Mathf.Clamp 
				(movementY, -maxSwayAmount, maxSwayAmount);

            //Slowly lerp to the sway position
            Vector3 finalSwayPosition = new Vector3 
				(movementX, movementY, 0);
			transform.localPosition = Vector3.Lerp 
				(transform.localPosition, finalSwayPosition + 
					initialSwayPosition, Time.deltaTime * swaySmoothValue);
        }
    }

    private void Update() 
    {
        if(Input.GetButton("Fire2") && !isReloading && !isRunning)
        {
            

            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, aimFov, fovSpeed * Time.deltaTime);

            isAiming = true;
            anim.SetBool("Aim", true);

            if (!soundHasPlayed)
            {
                mainAudioSource.clip = SoundClips.aimSound;
                mainAudioSource.Play();

                soundHasPlayed = true;
            }
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFov, fovSpeed * Time.deltaTime);

            isAiming = false;
            anim.SetBool("Aim", false);
        }

        if (randomMuzzleflash == true)
        {
            randomMuzzleflashValue = Random.Range(minRandomValue, maxRandomValue);
        }

        AnimationCheck();

        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(GrenadeSpawnDelay());
            anim.Play("GrenadeThrow", 0, 0.0f);
        }

        if (currentAmmo == 0)
        {
            outOfAmmo = true;

            if (autoReload == true && !isReloading)
            {
                StartCoroutine(AutoReload());
            }
        }
        else
        {
            outOfAmmo = false;
        }
        
        if(Input.GetButton("Fire1") && !outOfAmmo && !isReloading && !isRunning)
        {   
            print (currentAmmo < ammo);

            if (Time.time - lastFired > 1 /fireRate)
            {
                lastFired = Time.time;
                currentAmmo -=1;

                anim.SetBool("Fire", true);
                shootAudioSource.clip = SoundClips.shootSound;
                shootAudioSource.Play();

                if (!isAiming)
                {
                    anim.Play("Fire", 0, 0f);

                    if (!randomMuzzleflash && enableMuzzleflash == true)
                    {
                        muzzleParticles.Emit(1);
                        StartCoroutine(MuzzleFlashLight());
                    }
                    else if (randomMuzzleflash == true)
                    {
                        if (randomMuzzleflashValue == 1)
                        {
                            if (enableSparks == true)
                            {
                                sparkParticles.Emit(Random.Range(minSparkEmission, maxSparkEmission));
                            }
                            if (enableMuzzleflash == true)
                            {
                                muzzleParticles.Emit(1);
                                StartCoroutine(MuzzleFlashLight());
                            }
                        }
                    }
                }
                else 
                {
                    anim.Play("Aim Fire", 0, 0f);
                    if (!randomMuzzleflash)
                    {
                        muzzleParticles.Emit(1);
                    }
                    else if (randomMuzzleflash == true)
                    {
                        if (randomMuzzleflashValue == 1)
                        {
                            if (enableSparks == true)
                            {
                                sparkParticles.Emit(Random.Range(minSparkEmission, maxSparkEmission));
                            }
                            if (enableMuzzleflash == true)
                            {
                                muzzleParticles.Emit(1);
                                StartCoroutine(MuzzleFlashLight());
                            }
                        }
                    }
                }

                var bullet = (Transform)Instantiate(
                    Prefabs.bulletPrefab, 
                    Spawnpoints.bulletSpawnPoint.transform.position,
                    Spawnpoints.bulletSpawnPoint.transform.rotation);
                
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletForce;

                Instantiate(Prefabs.casingPrefab,
                Spawnpoints.casingSpawnPoint.transform.position, 
                Spawnpoints.casingSpawnPoint.transform.rotation);
            }
        }
        else
        {
            anim.SetBool("Fire", false);
        }

        if(Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            Reload();
        }

        //Walking when pressing down WASD keys
		if (Input.GetKey (KeyCode.W) && !isRunning || 
			Input.GetKey (KeyCode.A) && !isRunning || 
			Input.GetKey (KeyCode.S) && !isRunning || 
			Input.GetKey (KeyCode.D) && !isRunning) 
		{
			anim.SetBool ("Walk", true);
		} 
        else 
        {
			anim.SetBool ("Walk", false);
		}
        
        //Running
        if ((Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.LeftShift))) 
		{
			isRunning = true;
		} 
        else 
        {
			isRunning = false;
		}
		
		//Run anim toggle
		if (isRunning == true) 
		{
			anim.SetBool ("Run", true);
		} 
		else 
		{
			anim.SetBool ("Run", false);
		}
    }

    private IEnumerator GrenadeSpawnDelay() 
    {
		//Wait for set amount of time before spawning grenade
		yield return new WaitForSeconds (grenadeSpawnDelay);
		//Spawn grenade prefab at spawnpoint
		Instantiate(Prefabs.grenadePrefab, 
			Spawnpoints.grenadeSpawnPoint.transform.position, 
			Spawnpoints.grenadeSpawnPoint.transform.rotation);
	}

    private IEnumerator AutoReload()
    {
        yield return new WaitForSeconds(autoReloadDelay);

        if(outOfAmmo == true)
        {
            anim.Play("Reload Out of Ammo", 0, 0f);

            mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
			mainAudioSource.Play ();
        }
        currentAmmo = ammo;
        outOfAmmo = false;
    }

    private void Reload()
    {
        if (currentAmmo < ammo)
        {
            anim.Play("Reload Out Of Ammo", 0, 0f);

            mainAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
			mainAudioSource.Play ();

            currentAmmo = ammo;
            outOfAmmo = false;
        }
    }

    private IEnumerator MuzzleFlashLight () 
    {	
		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		muzzleflashLight.enabled = false;
	}

    private void AnimationCheck () 
    {
		
		//Check if reloading
		//Check both animations
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Out Of Ammo") || 
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Ammo Left")) 
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}
    }

}
