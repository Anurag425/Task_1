using UnityEngine;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using DG.Tweening;
using System.Collections;

public class Manager : MonoBehaviour
{
    public static Manager instance;


    [Serializable]
    public class Client
    {
        public bool isManager;
        public int id;
        public string label;
    }

    [Serializable]
    public class ClientInfo
    {
        public string address;
        public string name;
        public int points;
    }

    [Serializable]
    public class ClientData
    {
        public List<Client> clients;
        public Dictionary<int, ClientInfo> data;
        public string label;
    }

    private ClientData GetClientData()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://qa2.sunbasedata.com/sunbase/portal/api/assignment.jsp?cmd=client_data");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        Debug.Log(jsonResponse);
        ClientData clientData = JsonConvert.DeserializeObject<ClientData>(jsonResponse);
        string json = JsonConvert.SerializeObject(clientData);
        print(json);
        return clientData;
    }

    public GameObject dataBlock;
    public GameObject contentView;
    ClientData clientdata;
    List<GameObject> blocks = new List<GameObject>();
    public GameObject popupWindow;
    public TextMeshProUGUI popupName;
    public TextMeshProUGUI popupPoints;
    public TextMeshProUGUI popupAddress;
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    private void Start()
    {
        clientdata = GetClientData();
        foreach (var (id, clientInfo) in clientdata.data)
        {
            dataBlock.GetComponent<Block>().UpdateValues(clientdata.clients[id - 1].label, clientInfo.points, id);
            blocks.Add(Instantiate(dataBlock, contentView.transform));
        }

    }

    public void OnValueChanged(int idx)
    {
        foreach(var ele in blocks)
        {
            ele.GetComponent<Block>().DestroyBlock();
        }
        blocks.Clear();
        switch (idx)
        {
            case 0:
                foreach (var (id, clientInfo) in clientdata.data)
                {
                    dataBlock.GetComponent<Block>().UpdateValues(clientdata.clients[id - 1].label, clientInfo.points, id);
                    blocks.Add(Instantiate(dataBlock, contentView.transform));
                }
                break;

            case 1:
                foreach (var (id, clientInfo) in clientdata.data)
                {
                    if (clientdata.clients[id-1].isManager)
                    {
                        dataBlock.GetComponent<Block>().UpdateValues(clientdata.clients[id - 1].label, clientInfo.points, id);
                        blocks.Add(Instantiate(dataBlock, contentView.transform));
                    }
                }
                break;

            case 2:
                foreach (var (id, clientInfo) in clientdata.data)
                {
                    if (!clientdata.clients[id - 1].isManager)
                    {
                        dataBlock.GetComponent<Block>().UpdateValues(clientdata.clients[id - 1].label, clientInfo.points, id);
                        blocks.Add(Instantiate(dataBlock, contentView.transform));
                    }
                }
                break;
        }
    }

    public void ShowClientInfoPopup(int id)
    {
        popupName.text = clientdata.data[id].name;
        popupPoints.text = clientdata.data[id].points.ToString();
        popupAddress.text = clientdata.data[id].address;
        popupWindow.SetActive(true);
        popupWindow.transform.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.Linear);
    }

    public void CloseClientInfoPopup()
    {
        StartCoroutine("PopupClose");
    }

    private IEnumerator PopupClose()
    {
        popupWindow.transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
        popupWindow.SetActive(false);
    }
}
