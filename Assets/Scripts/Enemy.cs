using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float shootCounter;
    [SerializeField] float minTimeBetweenShot = .2f;
    [SerializeField] float maxTimeBetweenShot = 2f;
    [SerializeField] float projectileSpeed = -15f;
    [SerializeField] GameObject explosionVFX;
    [SerializeField] AudioClip destroySound;

    [SerializeField] GameObject laserPrefab;

    private void Start()
    {
        shootCounter = Random.Range(minTimeBetweenShot, maxTimeBetweenShot);
    }

    private void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        // Count down how much time passed since last shoot.
        // Update once every frame using deltaTime (so it will count real time).
        shootCounter -= Time.deltaTime;

        if(shootCounter <= 0f)
        {
            Fire();

            shootCounter = Random.Range(minTimeBetweenShot, maxTimeBetweenShot);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
            laserPrefab,
            transform.position,
            Quaternion.identity
        );

        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0,  projectileSpeed);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

        if (!damageDealer) return;

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        damageDealer.Hit();

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);

        TriggerExplosionVFX();

        AudioSource.PlayClipAtPoint(destroySound, Camera.main.transform.position);
    }

    private void TriggerExplosionVFX()
    {
        GameObject explosion = Instantiate(explosionVFX, transform.position, transform.rotation);

        Destroy(explosion, 1f);
    }
}
