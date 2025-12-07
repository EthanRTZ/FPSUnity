using UnityEngine;
using TMPro;


public class GunSystem : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    // SUITE : ajout d'un champ pour détecter les changements de magazineSize 
    int prevMagazineSize;


    //bools 
    bool shooting, readyToShoot, reloading;


    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;


    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public TextMeshProUGUI text;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        prevMagazineSize = magazineSize; // <- initialisation de la valeur précédente
        UpdateAmmoText(); // <- initialisation de l'affichage
    }
    private void Update()
    {
        MyInput();


        // Si le magazineSize change (via Inspector ou code), adapter bulletsLeft et le texte
        if (magazineSize != prevMagazineSize)
        {
            // S'assurer que bulletsLeft ne dépasse pas le nouveau magazineSize
            bulletsLeft = Mathf.Clamp(bulletsLeft, 0, magazineSize);
            prevMagazineSize = magazineSize;
            UpdateAmmoText();
        }

        //SetText
        // suppression de la mise à jour par frame pour ne mettre à jour que lorsque nécessaire
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);


        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();


        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;


        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);


        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward;
        direction += fpsCam.transform.right * Random.Range(-spread, spread);
        direction += fpsCam.transform.up * Random.Range(-spread, spread);
        direction.Normalize();



        // Raycast unique : si on touche quelque chose on gère les dégâts + le bullet hole
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {
            Debug.Log("Hit: " + rayHit.collider.name);


            // Vérifie si l'objet touché est dans le LayerMask whatIsEnemy
            int hitLayer = rayHit.collider.gameObject.layer;
            if ((whatIsEnemy.value & (1 << hitLayer)) != 0)
            {
                var rd = rayHit.collider.GetComponentInParent<ReceiveDamage>();
                if (rd != null)
                {
                    rd.GetDamage(damage);
                }
            }


            // Place le trou de balle si défini
            if (bulletHoleGraphic != null)
                Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        }


        // Muzzle flash (si défini)
        if (muzzleFlash != null && attackPoint != null)
        {
            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation);
            var ps = flash.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }


        bulletsLeft--;
        bulletsShot--;

        UpdateAmmoText(); // <- mise à jour après tir

        Invoke("ResetShot", timeBetweenShooting);


        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        if (text != null) text.SetText("Reloading..."); // affiche l'état de rechargement
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        UpdateAmmoText(); // <- mise à jour après fin de rechargement
    }

    // Nouvelle méthode pour centraliser l'affichage des munitions
    private void UpdateAmmoText()
    {
        if (text == null) return;
        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    // OnValidate est appelé dans l'éditeur quand une valeur change dans l'Inspector
    private void OnValidate()
    {
        // Eviter erreurs hors jeu : mettre à jour prevMagazineSize et bulletsLeft en conséquence
        if (!Application.isPlaying)
        {
            prevMagazineSize = magazineSize;
            bulletsLeft = Mathf.Clamp(bulletsLeft, 0, magazineSize);
            if (text != null) text.SetText(bulletsLeft + " / " + magazineSize);
        }
    }

    // Méthode publique optionnelle pour changer dynamiquement la taille du chargeur par code
    public void SetMagazineSize(int newSize, bool refillToMax = false)
    {
        if (newSize < 1) newSize = 1;
        magazineSize = newSize;
        prevMagazineSize = magazineSize;
        if (refillToMax) bulletsLeft = magazineSize;
        else bulletsLeft = Mathf.Clamp(bulletsLeft, 0, magazineSize);
        UpdateAmmoText();
    }
}
