using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class StickHandTestScript
    {
        private const string mainSceneName = "Scenes/TrialScene";
        private GameObject gameGameObject;
        private JoyStick m_JoyStick;

        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene(mainSceneName);
        }

        [TearDown]
        public void Teardown()
        {
            SceneManager.UnloadSceneAsync(mainSceneName);
        }

        [UnityTest]
        public IEnumerator TestStickHandInited()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject m_StickHand = GameObject.Find("StickHand");
            Assert.AreEqual(m_StickHand.gameObject.GetComponent<Renderer>().enabled, false);
        }

        [UnityTest]
        public IEnumerator TestAimObjectInited()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject m_AimObject = GameObject.Find("AimObject");
            Assert.AreEqual(m_AimObject.gameObject.GetComponent<Renderer>().enabled, false);
        }

        [UnityTest]
        public IEnumerator TestStartButtonInited()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject m_StartButton = GameObject.Find("StartButton");
            Assert.AreEqual(m_StartButton.gameObject.activeSelf, true);
        }

        [UnityTest]
        public IEnumerator TestBackButtonInited()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject m_BackButton = GameObject.Find("BackButton");
            Assert.AreEqual(m_BackButton.gameObject.activeSelf, true);
        }
    }
}
