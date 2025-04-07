using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineHelper;
using UnityEngine.UI;

namespace DesignPattern
{
    #region [Facade Pattern]
    //Facade Pattern

    public interface IWindowFacadePattern
    {
        public void OpenWindow();
        public void CloseWindow();
    }

    public interface UIFacadePattern
    {
        public void WindowOpenOrClose(WindowState state);
        public void btnColorChange();
        public void PushColor(Button button);
        public void InitColor(Button button);
    }


    #endregion [Facade Pattern]

    #region [Composite Pattern]
    //Composite Pattern
    public interface ICompositePattern
    {
        public void Operate();
    }


    #endregion [Composite Pattern]

    #region [Adapter Pattern]
    //Adapter Pattern


    #endregion [Adapter Pattern]

    #region [Observer Pattern]
    //Observer Pattern

    public interface IObserver
    {
        void Notify(); // 옵저버들이 수행할 메서드
    }

    public interface ISubject
    {
        void RegisterObserver(IObserver observer); // 등록
        void RemoveObserver(IObserver observer); // 해제
        void NotifyObservers(); // 옵저버들 수행
    }

    public interface IUIObserver
    {
        void Notify(UserData userdata);
    }

    public interface IUISubject
    {
        void RegisterObserver(IUIObserver observer); // 등록
        void RemoveObserver(IUIObserver observer); // 해제
        void NotifyObservers(UserData userdata); // 옵저버들 수행
    }


    #endregion [Observer Pattern]

    #region [State Pattern & FSM]
    public interface IState<T>
    {
        void EnterState(T obj); // 상태 진입 시 호출
        void UpdateState(T obj); // 상태 업데이트

        void ExitState(T obj);   // 상태 종료 시 호출
    }

    #endregion [State Pattern & FSM]

    #region [Scene Initializeable]
    public interface ISceneInitializable
    {
        IEnumerator InitializeScene();
        IEnumerator InitializeAddtive();
    }


    #endregion [Scene Initializable]
}
