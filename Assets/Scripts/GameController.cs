
using UnityEngine;
using UnityEngine.InputSystem;  


public class GameController : MonoBehaviour
{
    enum GameState{start, game, end};
    [SerializeField] CursorController cursorController;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject endPanel;
    GameState currentState;
    
    int missCount;
    
    void Start()
    {
        currentState = GameState.start;
        gamestart();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == GameState.start)
        {
          
            if(Keyboard.current.enterKey.wasPressedThisFrame){
                 currentState = GameState.game;
                 gaming();
                  print("let's game");
             }
        }else if(currentState == GameState.game)
        {
            gaming();
             if (missCount >= 3)
            {
               
               currentState = GameState.end;
               gameend();
                print("let's end");
            }
        } else if(currentState == GameState.end)
        {
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame){
                 currentState = GameState.start;
                gamestart();
                 print("start!");
            }
        }
    }

    void gamestart()
    {
        missCount = 0;
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endPanel.SetActive(false);
        cursorController.gameObject.SetActive(false);
        
    }

    void gaming()
    {
         if (Keyboard.current.spaceKey.wasPressedThisFrame) {missCount += 1;}
         startPanel.SetActive(false);
         gamePanel.SetActive(true);
         endPanel.SetActive(false);
         cursorController.gameObject.SetActive(true);
        //change the condition to numbers
        
    }

    void gameend()
    {
         startPanel.SetActive(false);
         gamePanel.SetActive(false);
         endPanel.SetActive(true);
         cursorController.gameObject.SetActive(false);
         
    }
}
