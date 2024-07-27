using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class game_manager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float songOffset;
    [SerializeField] float songBPM;
    [SerializeField] double beatTime;

    // GameObjects
    [SerializeField] GameObject[] spawnpoints;
    [SerializeField] GameObject cube;

    private Light2D globalLight;
    private Light2D playerSpotLight;
    private bool started = false;
    private double initialTime;
    private double realInitialTime;
    private float initialIntensity;
    private float initialGlobalIntensity;
    private float initialOuterRadius;
    public double adjustedOffset;
    public int beatOffset;
    private double beatOffsetToTime;
    public int objectNumber;
    private bool dropmode;

    // debug
    public float now;
    private float initialTimeTime;

    

    void Start()
    {
        globalLight = GameObject.FindWithTag("GlobalLight").GetComponent<Light2D>();
        playerSpotLight = GameObject.FindWithTag("PlayerLight").GetComponent<Light2D>();
        beatTime = 60 / songBPM;
        initialTime = AudioSettings.dspTime;
        realInitialTime = AudioSettings.dspTime;
        initialIntensity = playerSpotLight.intensity;
        initialGlobalIntensity = globalLight.intensity;
        initialOuterRadius = playerSpotLight.pointLightOuterRadius;
        beatOffsetToTime = beatOffset * beatTime;
        objectNumber = 0;
    }

    IEnumerator BeatPlayerGlow(float fadeoutTime, float impactIntensity, float impactOuterRadius){
        playerSpotLight.intensity = impactIntensity;
        playerSpotLight.pointLightOuterRadius = impactOuterRadius;
        for (int i = 0; i < 5; i++){
            playerSpotLight.intensity -= (impactIntensity - initialIntensity) / 5;
            playerSpotLight.pointLightOuterRadius -= (impactOuterRadius - initialOuterRadius) / 5;
            yield return new WaitForSeconds(fadeoutTime / 5);
        }
    }

    IEnumerator BeatGlobalGlow(float fadeoutTime, float impactIntensity){
        globalLight.intensity = impactIntensity;

        for (int i = 0; i < 5; i++){
            globalLight.intensity -= (impactIntensity - initialGlobalIntensity) / 5;
            yield return new WaitForSeconds(fadeoutTime / 5);
        }
    }

    IEnumerator ShootCube(int objnum, int pos, float strength, Vector2 force, float angleVel, float xsize, float ysize, Color color, double activateTime){
        if (objnum == objectNumber && AudioSettings.dspTime >= activateTime + realInitialTime + adjustedOffset + beatOffsetToTime){
            GameObject summoned_cube = Instantiate(cube);
            Rigidbody2D cube_rb = summoned_cube.GetComponent<Rigidbody2D>();
            summoned_cube.transform.position = spawnpoints[pos].transform.position;
            cube_rb.angularVelocity = angleVel;
            summoned_cube.transform.localScale = new Vector3(xsize, ysize, 1);
            summoned_cube.GetComponent<SpriteRenderer>().color = color;
            cube_rb.AddForce(force * strength, ForceMode2D.Impulse);

            objectNumber += 1;
        }
        
        yield return null;
    }

    void Update()
    {
        now = Time.time - initialTimeTime;

        if (Input.GetKeyDown(KeyCode.Return)){
            // reset settings
            initialTime = AudioSettings.dspTime;
            realInitialTime = AudioSettings.dspTime;
            objectNumber = 0;
            dropmode = false;

            initialTimeTime = Time.time;

            started = true;
            audioSource.Stop();
            audioSource.time = songOffset;
            audioSource.Play();
        }

        if (started){
            if (AudioSettings.dspTime >= initialTime + beatTime + adjustedOffset + beatOffsetToTime){
                initialTime += beatTime;
                StartCoroutine(BeatPlayerGlow(0.2f, 1f, 1.8f));
                StartCoroutine(BeatGlobalGlow(0.2f, 0.01f));

                if (dropmode){
                    Color randomColor = new Color(Random.value, Random.value, Random.value);
                    float randomSize = Random.Range(0.1f, 0.7f);
                    float falloffVal = Random.Range(0.5f, 0.7f);

                    GameObject summoned_cube = Instantiate(cube);
                    summoned_cube.transform.position = new Vector3(Random.Range(-8.24f, 8.24f), 4.23f, 0);
                    summoned_cube.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-150, 150);
                    summoned_cube.transform.localScale = new Vector3(randomSize, randomSize, 1);
                    summoned_cube.transform.GetChild(0).GetComponent<Light2D>().color = randomColor;
                    summoned_cube.transform.GetChild(0).GetComponent<Light2D>().intensity = Random.Range(30f, 90f);
                    summoned_cube.transform.GetChild(0).GetComponent<Light2D>().falloffIntensity = falloffVal;
                }
            }

            // Patterns
            // Drop: 2.4s 5.2s 8.0s 10.8s (gap: 2.7s)
            
            // 1bar | start: 0.47
            if (AudioSettings.dspTime >= 0.47 + realInitialTime + adjustedOffset + beatOffsetToTime && !dropmode){
                dropmode = true;
            }
            // else if (AudioSettings.dspTime >= 0.47 + realInitialTime + adjustedOffset + beatOffsetToTime)

            StartCoroutine(ShootCube(0, 0, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 0.47f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(1, 1, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 0.47f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(2, 2, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 0.47f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(3, 3, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 0.47f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(4, 4, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 0.47f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(5, 4, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 3.17f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(6, 3, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 3.17f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(7, 2, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 3.17f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(8, 1, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 3.17f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(9, 0, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 3.17f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(10, 0, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.cyan, 5.87f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(11, 4, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.cyan, 5.87f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(12, 1, 10, new Vector2(0.07f, 1f), 40, 0.5f, 0.5f, Color.cyan, 5.87f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(13, 3, 10, new Vector2(-0.07f, 1f), 40, 0.5f, 0.5f, Color.cyan, 5.87f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(14, 2, 10, new Vector2(0f, 1f), 40, 0.5f, 0.5f, Color.cyan, 5.87f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(15, 2, 10, new Vector2(0f, 1f), 40, 0.5f, 0.5f, Color.magenta, 8.57f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(16, 1, 10, new Vector2(0.07f, 1f), 40, 0.5f, 0.5f, Color.magenta, 8.57f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(17, 3, 10, new Vector2(-0.07f, 1f), 40, 0.5f, 0.5f, Color.magenta, 8.57f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(18, 0, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.magenta, 8.57f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(19, 4, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.magenta, 8.57f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(20, 0, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 11.27f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(21, 1, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 11.27f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(22, 2, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 11.27f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(23, 3, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 11.27f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(24, 4, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.yellow, 11.27f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(25, 4, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 13.97f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(26, 3, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 13.97f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(27, 2, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 13.97f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(28, 1, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 13.97f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(29, 0, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.green, 13.97f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(30, 0, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.cyan, 16.67f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(31, 4, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.cyan, 16.67f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(32, 1, 10, new Vector2(0.07f, 1f), 40, 0.5f, 0.5f, Color.cyan, 16.67f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(33, 3, 10, new Vector2(-0.07f, 1f), 40, 0.5f, 0.5f, Color.cyan, 16.67f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(34, 2, 10, new Vector2(0f, 1f), 40, 0.5f, 0.5f, Color.cyan, 16.67f + beatTime / 2 * 4));

            StartCoroutine(ShootCube(35, 2, 10, new Vector2(0f, 1f), 40, 0.5f, 0.5f, Color.magenta, 19.37f + beatTime / 2 * 0));
            StartCoroutine(ShootCube(36, 1, 10, new Vector2(0.07f, 1f), 40, 0.5f, 0.5f, Color.magenta, 19.37f + beatTime / 2 * 1));
            StartCoroutine(ShootCube(37, 3, 10, new Vector2(-0.07f, 1f), 40, 0.5f, 0.5f, Color.magenta, 19.37f + beatTime / 2 * 2));
            StartCoroutine(ShootCube(38, 0, 10, new Vector2(0.15f, 1f), 40, 0.5f, 0.5f, Color.magenta, 19.37f + beatTime / 2 * 3));
            StartCoroutine(ShootCube(39, 4, 10, new Vector2(-0.15f, 1f), 40, 0.5f, 0.5f, Color.magenta, 19.37f + beatTime / 2 * 4));
        }
        
    }
}
