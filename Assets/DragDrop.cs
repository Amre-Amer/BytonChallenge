using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour {
    List<GameObject> songs = new List<GameObject>();
    List<GameObject> contacts = new List<GameObject>();
    List<GameObject> shortcuts = new List<GameObject>();
    List<GameObject> playlist = new List<GameObject>();
    List<GameObject> trash = new List<GameObject>();
    GameObject itemSong;
    GameObject itemContact;
    GameObject itemShortcut;
    GameObject itemPlaylist;
    GameObject itemSelected;
//    GameObject itemOriginal;
    GameObject itemOriginalFrom;
    GameObject itemOriginalTo;
    GameObject scrollviewSelected;
    GameObject scrollviewOriginal;
    List<GameObject> itemsSelected;
    Canvas canvas;
    GameObject scrollviewSongs;
    GameObject scrollviewContacts;
    GameObject scrollviewShortcuts;
    GameObject scrollviewPlaylist;
    GameObject scrollviewTrash;
    GameObject goShared;
    AudioSource audioSource;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        scrollviewSongs = GameObject.Find("ScrollviewSongs");
        scrollviewContacts = GameObject.Find("ScrollviewContacts");
        scrollviewShortcuts = GameObject.Find("ScrollviewShortcuts");
        scrollviewPlaylist = GameObject.Find("ScrollviewPlaylist");
        scrollviewTrash = GameObject.Find("ScrollviewTrash");
        itemSong = GameObject.Find("ItemSong");
        itemContact = GameObject.Find("ItemContact");
        itemShortcut = GameObject.Find("ItemShortcut");
        itemPlaylist = GameObject.Find("ItemPlaylist");
        goShared = GameObject.Find("TextShared");
        goShared.GetComponentInChildren<Text>().enabled = false;
        LoadSongs();
        LoadContacts();
    }

    // Update is called once per frame
    void Update () {
        UpdateTouch();
	}

    void LoadContacts()
    {
        AddContact("Contact 1");
        AddContact("Contact 2");
        AddContact("Contact 3");
        AddContact("Contact 4");
    }

    void LoadSongs()
    {
        AddSong("Song A");
        AddSong("Song B");
        AddSong("Song C");
        AddSong("Song D");
    }

    void AddContact(string txt)
    {
        if (scrollviewOriginal == null)
        {
            AddToScrollview(scrollviewContacts, contacts, itemContact, txt);
        }
    }

    void AddSong(string txt)
    {
        if (scrollviewOriginal == null)
        {
            AddToScrollview(scrollviewSongs, songs, itemSong, txt);
        }
    }

    void AddShortcut(string txt)
    {
        RemoveItemFromScrollview(scrollviewShortcuts, shortcuts, txt);
        AddToScrollview(scrollviewShortcuts, shortcuts, itemShortcut, txt);
    }

    void AddPlaylist(string txt)
    {
        if (scrollviewOriginal == scrollviewSongs)
        {
            RemoveItemFromScrollview(scrollviewPlaylist, playlist, txt);
            AddToScrollview(scrollviewPlaylist, playlist, itemPlaylist, txt);
        }
    }

    void AddTrash(string txt)
    {
        itemsSelected.Remove(itemOriginalFrom);
        DestroyImmediate(itemOriginalFrom);
        if (scrollviewOriginal == scrollviewSongs || scrollviewOriginal == scrollviewContacts)
        {
            RemoveItemFromScrollview(scrollviewShortcuts, shortcuts, txt);
            RemoveItemFromScrollview(scrollviewPlaylist, playlist, txt);
        }
    }

    void RemoveItemFromScrollview(GameObject scrollview, List<GameObject>items, string txt)
    {
        GameObject content = scrollview.transform.Find("Viewport/Content").gameObject;
        foreach (Transform t in content.transform)
        {
            GameObject go = t.gameObject;
            Text text = go.GetComponentInChildren<Text>();
            if (text.text == txt)
            {
                items.Remove(go);
                DestroyImmediate(go);
            }
        }
    }

    void AddToScrollview(GameObject scrollview, List<GameObject>items, GameObject itemOriginal, string txt)
    {
        GameObject content = scrollview.transform.Find("Viewport/Content").gameObject;
        GameObject item = Instantiate(itemOriginal, content.transform);
        Text text = item.GetComponentInChildren<Text>();
        text.text = txt;
        items.Add(item);
    }

    void ShareSongWithContact()
    {
        goShared.GetComponentInChildren<Text>().enabled = true;
        string txtShared = "\"Shared " + itemOriginalFrom.GetComponentInChildren<Text>().text + " with " + itemOriginalTo.GetComponentInChildren<Text>().text + "\"";
        goShared.GetComponentInChildren<Text>().text = txtShared;
        audioSource.Play();
        itemOriginalTo.GetComponentInChildren<Image>().color += Color.green / 6;
        Invoke("HideSharedMessage", 2);
    }

    void HideSharedMessage()
    {
        goShared.GetComponentInChildren<Text>().enabled = false;
        itemOriginalTo.GetComponentInChildren<Image>().color -= Color.green / 6;
    }

    void UpdateTouch()
    {
        bool ynReturnCopy = true;
        if (Input.GetMouseButtonDown(0) == true)
        {
            itemSelected = GetItemUnderMouse(ynReturnCopy);
            scrollviewOriginal = GetScrollviewUnderMouse();
        }
        if (Input.GetMouseButtonUp(0) == true)
        {
            GetItemUnderMouse(false);
            scrollviewSelected = GetScrollviewUnderMouse();
            if (scrollviewSelected != null)
            {
                string txt = itemSelected.GetComponentInChildren<Text>().text;
                if (scrollviewSelected == scrollviewContacts)
                {
                    string txtOrig = itemOriginalFrom.GetComponentInChildren<Text>().text;
                    if (scrollviewOriginal != scrollviewContacts && txtOrig.Contains("Song"))
                    {
                        if (itemOriginalTo != null)
                        {
                            ShareSongWithContact();
                        }
                    }
                    else
                    {
                        AddContact(txt);
                    }
                }
                if (scrollviewSelected == scrollviewSongs)
                {
                    AddSong(txt);
                }
                if (scrollviewSelected == scrollviewTrash)
                {
                    AddTrash(txt);
                }
                if (scrollviewSelected == scrollviewShortcuts)
                {
                    AddShortcut(txt);
                }
                if (scrollviewSelected == scrollviewPlaylist)
                {
                    if (scrollviewOriginal == scrollviewPlaylist)
                    {
                        Reorder();
                    }
                    else
                    {
                        AddPlaylist(txt);
                    }
                }
            }
            DestroyImmediate(itemSelected);
        }
        if (itemSelected != null)
        {
            itemSelected.transform.position = Input.mousePosition;
        }
    }

    void Reorder()
    {
        int n = itemOriginalFrom.transform.GetSiblingIndex();
        itemOriginalFrom.transform.SetSiblingIndex(itemOriginalTo.transform.GetSiblingIndex());
        itemOriginalTo.transform.SetSiblingIndex(n);
    }

    GameObject GetItemUnderMouse(bool ynReturnCopy)
    {
        GameObject go = null;
        if (go == null)
        {
            go = GetItemUnderMouseForItems(songs, ynReturnCopy);
        }
        if (go == null)
        {
            go = GetItemUnderMouseForItems(contacts, ynReturnCopy);
        }
        if (go == null)
        {
            go = GetItemUnderMouseForItems(shortcuts, ynReturnCopy);
        }
        if (go == null)
        {
            go = GetItemUnderMouseForItems(playlist,  ynReturnCopy);
        }
        return go;
    }

    GameObject GetItemUnderMouseForItems(List<GameObject> items, bool ynReturnCopy)
    {
        foreach (GameObject item in items)
        {
            if (IsMouseOverGo(item) == true)
            {
                itemsSelected = items;
                if (ynReturnCopy == true)
                {
                    itemOriginalFrom = item;
                    GameObject goCopy = Instantiate(item, canvas.transform);
                    return goCopy;
                }
                else
                {
                    itemOriginalTo = item;
                    return item;
                }
            }
        }
        return null;
    }

    GameObject GetScrollviewUnderMouse()
    {
        if (IsMouseOverGo(scrollviewSongs) == true)
        {
            return scrollviewSongs;
        }
        if (IsMouseOverGo(scrollviewContacts) == true)
        {
            return scrollviewContacts;
        }
        if (IsMouseOverGo(scrollviewShortcuts) == true)
        {
            return scrollviewShortcuts;
        }
        if (IsMouseOverGo(scrollviewTrash) == true)
        {
            return scrollviewTrash;
        }
        if (IsMouseOverGo(scrollviewPlaylist) == true)
        {
            return scrollviewPlaylist;
        }
        return null;
    }

    bool IsMouseOverGo(GameObject go)
    {
        float w = go.GetComponent<RectTransform>().sizeDelta.x;
        float h = go.GetComponent<RectTransform>().sizeDelta.y;
        float x = go.transform.position.x - w/2;
        float y = go.transform.position.y - h/2;
        Rect rect = new Rect(x, y, w, h);
        if (rect.Contains(Input.mousePosition) == true)
        {
            return true;
        }
        return false;
    }
}
