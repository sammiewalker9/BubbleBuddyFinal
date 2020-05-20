using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float PIPE_WIDTH = 0.15f;
    private const float PIPE_HEAD_HEIGHT = 0.08f;
    private const float CAMERA_ORTHO_SIZE = 0.75f;
    private const float PIPE_MOVE_SPEED = 0.45f;
    private const float PIPE_DESTROY_X_POSITION = -1.5f;
    private const float PIPE_SPAWN_X_POSITION = 1.6f;
    private const float FISH_X_POSITION = 0f;

    private static Level instance;


    public static Level GetInstance()
    {
        return instance; 
    }

    private List<Pipe> pipeList;
    private int pipesPassedCount;
    private int pipesSpawned; 
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        FishDead,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1.4f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start()
    {
        Fish.GetInstance().OnDied += Fish_OnDied;
        Fish.GetInstance().OnStartedPlaying += Fish_OnStartedPlaying;
    }

    private void Fish_OnStartedPlaying(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void Fish_OnDied(object sender, System.EventArgs e)
    {
        state = State.FishDead;
    }


    private void Update()
    {
        if(state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();

        }
       
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            //Time to spawn another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 0.15f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);

        }
    }


    private void HandlePipeMovement()
    {
        for (int i = 0; i<pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightOfFish = pipe.GetXPosition() > FISH_X_POSITION;
            pipe.Move();

            if(isToTheRightOfFish && pipe.GetXPosition() <= FISH_X_POSITION && pipe.isBottom())
            {
                // pipe passed fish
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);


            }
            if (pipe.GetXPosition() < PIPE_DESTROY_X_POSITION)
            {
                // Destroy Pipe
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }

        
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 0.55f;
                pipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty.Medium:
                gapSize = 0.48f;
                pipeSpawnTimerMax = 1.4f;
                break;
            case Difficulty.Hard:
                gapSize = 0.40f;
                pipeSpawnTimerMax = 1.6f;
                break;
            case Difficulty.Impossible:
                gapSize = 0.3f;
                pipeSpawnTimerMax = 1.9f;
                break;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawned >= 30) return Difficulty.Impossible;
        if (pipesSpawned >= 20) return Difficulty.Hard;
        if (pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

     
    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize * 0.5f, xPosition, false);
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        // Set up pipe head 
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadyPosition;
        if (createBottom)
        {
            pipeHeadyPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * .5f;
        }
        else
        {
            pipeHeadyPosition = + CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * .5f;

        }
        pipeHead.position = new Vector3(xPosition, pipeHeadyPosition);
        

        // Set up pipe body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }
        else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * .5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }


    public int getPipesPassedCount()
    {
        return pipesPassedCount;
    }

    // Represents a single pipe

    private class Pipe
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;
        private bool createBottom;


        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool createBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.createBottom = createBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }

        public bool isBottom()
        {
            return createBottom; // this is a bottom
        }

        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }

    }



}
