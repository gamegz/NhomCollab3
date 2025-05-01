using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SharedSectionResetLink
{
    public SectionReset sectionResetRef;
    public int extraRoomIndex;
}

[System.Serializable]
public class RoomMain
{
    public GameObject RoomTrigger1;
    public GameObject RoomTrigger2;
    public GameObject RoomTrigger3;
    public GameObject RoomTrigger4;
    // public GameObject Door1;
    // public GameObject Door2;
    // public SectionReset sharedSectionResetRef;
    // public int sharedExtraRoomIndex;
    public List<SharedSectionResetLink> sharedSectionResetLinks;
}

[System.Serializable]
public class ExtraRoom
{
    public List<GameObject> RoomTrigger;

    public bool WasCleared;
    public bool WasResetSinceCleared;
    

    public void Reset()
    {
        foreach (GameObject roomTrigger in RoomTrigger)
        {
            if (roomTrigger != null)
            {
                BoxCollider boxCollider = roomTrigger.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
            }
        }
        if (WasCleared)
        {
            WasResetSinceCleared = true;
        }
    }

    public void MarkAsCleared()
    {
        WasCleared = true;
        WasResetSinceCleared = false;
        foreach (GameObject roomTrigger in RoomTrigger)
        {
            UnResetRoomTriggerAndMarkClear(roomTrigger);
        }
    }

    public void UnResetToCleared()
    {
        foreach (GameObject roomTrigger in RoomTrigger)
        {
            UnResetRoomTriggerAndMarkClear(roomTrigger);
        }
        WasResetSinceCleared = false;
    }

    private void UnResetRoomTriggerAndMarkClear(GameObject roomTrigger)
    {
        if (RoomTrigger != null)
        {
            BoxCollider boxCollider = roomTrigger.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
    }
}
public class SectionReset : MonoBehaviour, IInteractable
{
    public List<RoomMain> MainRoomList;
    public List<ExtraRoom> ExtraRoomList;
    
    [SerializeField] private GameObject holdEText;
    [SerializeField] private GameObject pressEText;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.isRespawnPointClaimed(this.gameObject))
        {
            pressEText.SetActive(true);
                
            return;
        }
        else
        {
            holdEText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pressEText.SetActive(false);
        holdEText.SetActive(false);
    }

    public void ResetRoomAfterTeleport()
    {
        foreach (RoomMain room in MainRoomList)
        {
            
            ResetMainRoom(room.RoomTrigger1);
            ResetMainRoom(room.RoomTrigger2);
            ResetMainRoom(room.RoomTrigger3);
            ResetMainRoom(room.RoomTrigger4);
            
            foreach (var link in room.sharedSectionResetLinks)
            {
                if (link.sectionResetRef != null)
                {
                    var extraRooms = link.sectionResetRef.ExtraRoomList;
                    if (link.extraRoomIndex >= 0 && link.extraRoomIndex < extraRooms.Count)
                    {
                        var extraRoom = extraRooms[link.extraRoomIndex];
                        if (extraRoom.WasCleared)
                        {
                            extraRoom.WasResetSinceCleared = true;
                        }
                    }
                }
            }
            
        }

        foreach (ExtraRoom extraRoom in ExtraRoomList)
        {
            if (extraRoom.WasCleared && extraRoom.WasResetSinceCleared)
            {
                extraRoom.UnResetToCleared();
            }
            else if (!extraRoom.WasCleared)
            {
                extraRoom.Reset();
            }
        }
    }

    private void ResetMainRoom(GameObject roomTrigger)
    {
        if (roomTrigger != null)
        {
            BoxCollider boxCollider = roomTrigger.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
        }
    }

    public void OnInteract()
    {
        if (GameManager.Instance.isRespawnPointClaimed(this.gameObject))
        {
            return;
        }
        
        holdEText.SetActive(false);
        pressEText.SetActive(true);
        GameManager.Instance.ClaimRespawnPoimt(this.gameObject);
    }
}
