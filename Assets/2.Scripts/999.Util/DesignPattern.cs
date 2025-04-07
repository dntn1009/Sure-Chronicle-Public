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
        void Notify(); // ���������� ������ �޼���
    }

    public interface ISubject
    {
        void RegisterObserver(IObserver observer); // ���
        void RemoveObserver(IObserver observer); // ����
        void NotifyObservers(); // �������� ����
    }

    public interface IUIObserver
    {
        void Notify(UserData userdata);
    }

    public interface IUISubject
    {
        void RegisterObserver(IUIObserver observer); // ���
        void RemoveObserver(IUIObserver observer); // ����
        void NotifyObservers(UserData userdata); // �������� ����
    }


    #endregion [Observer Pattern]

    #region [State Pattern & FSM]
    public interface IState<T>
    {
        void EnterState(T obj); // ���� ���� �� ȣ��
        void UpdateState(T obj); // ���� ������Ʈ

        void ExitState(T obj);   // ���� ���� �� ȣ��
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
