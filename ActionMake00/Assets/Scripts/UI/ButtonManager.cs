using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    public void CloseUI()
    {
        GameObject UI = this.gameObject.transform.parent.gameObject;
        UI.SetActive(false);
    }

    public void HideMousePoint()
    {
        MouseControl.instance.Apply(MouseControl.AimCursorMode.LockedCenter);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        //타임스케일을 관리하는 이유는 혹여나 예상 외의 상황을 방지학기 위함.
        //기본적으로 모든 씬의 시작은 값이 1이기 때문이다
        Time.timeScale = 1f;                            
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
