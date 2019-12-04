using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Config
    [SerializeField] float health = 100;
    [SerializeField] float minTimeBetweenShot = .2f;
    [SerializeField] float maxTimeBetweenShot = 2f;
    [SerializeField] float projectileSpeed = -15f;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = .75f;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = .25f;
    [SerializeField] int scorePoints = 100;

    float shootCounter;

    // VFX
    [SerializeField] GameObject explosionVFX;
    [SerializeField] AudioClip destroySFX;
    [SerializeField] AudioClip shootSFX;

    // Prefabs
    [SerializeField] GameObject laserPrefab;

    // References
    GameSession gameSession;

    private void Start()
    {
        shootCounter = Random.Range(minTimeBetweenShot, maxTimeBetweenShot);

        gameSession = FindObjectOfType<GameSession>();
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

        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSoundVolume);
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

        AudioSource.PlayClipAtPoint(destroySFX, Camera.main.transform.position, deathSoundVolume);

        gameSession.AddToScore(scorePoints);
    }

    private void TriggerExplosionVFX()
    {
        GameObject explosion = Instantiate(explosionVFX, transform.position, transform.rotation);

        Destroy(explosion, 1f);
    }
}
