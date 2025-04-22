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
            Debug.Log("work ?");
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
                Debug.Log("Work or not?");
                boxCollider.enabled = false;
            }
        }
    }
}
public class SectionReset : MonoBehaviour, IInteractable
{
    public List<RoomMain> MainRoomList;
    public List<ExtraRoom> ExtraRoomList;
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        foreach (Room room in RoomList)
    //        {
    //            if (room.RoomTrigger != null)
    //            {
    //                BoxCollider boxCollider = room.RoomTrigger.GetComponent<BoxCollider>();
    //                if (boxCollider != null)
    //                {
    //                    boxCollider.enabled = true;
    //                }
    //            }

    //            if (room.EntranceDoor != null)
    //            {
    //                room.EntranceDoor.SetActive(false);
    //            }

    //            if (room.ExitDoor != null)
    //            {
    //                room.ExitDoor.SetActive(true);
    //            }
    //        }
    //    }
    //}

    public void ResetRoomAfterTeleport()
    {
        foreach (RoomMain room in MainRoomList)
        {
            /*if (room.RoomTrigger1 != null)
            {
                BoxCollider boxCollider = room.RoomTrigger1.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
            }

            if (room.RoomTrigger2 != null)
            {
                BoxCollider boxCollider = room.RoomTrigger2.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
            }

            if (room.RoomTrigger3 != null)
            {
                BoxCollider boxCollider = room.RoomTrigger3.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
            }

            if (room.RoomTrigger4 != null)
            {
                BoxCollider boxCollider = room.RoomTrigger4.GetComponent<BoxCollider>();
                if (boxCollider != null)
                {
                    boxCollider.enabled = true;
                }
            }*/
            ResetMainRoom(room.RoomTrigger1);
            ResetMainRoom(room.RoomTrigger2);
            ResetMainRoom(room.RoomTrigger3);
            ResetMainRoom(room.RoomTrigger4);
            
            // if (room.sharedSectionResetRef != null)
            // {
            //     var extraRoom = room.sharedSectionResetRef.ExtraRoomList[room.sharedExtraRoomIndex];
            //     if (extraRoom.WasCleared)
            //     {
            //         extraRoom.WasResetSinceCleared = true;
            //         Debug.Log("Synced WasResetSinceCleared to shared ExtraRoom");
            //     }
            // }
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
                            Debug.Log("Synced WasResetSinceCleared to shared ExtraRoom");
                        }
                    }
                }
            }

            // if (room.Door1 != null)
            // {
            //     room.Door1.SetActive(false);
            // }
            //
            // if (room.ExitDoor != null)
            // {
            //     room.ExitDoor.SetActive(true);
            // }
        }

        foreach (ExtraRoom extraRoom in ExtraRoomList)
        {
            Debug.Log(extraRoom);
            Debug.Log(extraRoom.WasCleared);
            Debug.Log(extraRoom.WasResetSinceCleared);
            if (extraRoom.WasCleared && extraRoom.WasResetSinceCleared)
            {
                Debug.Log("ever work here ?");
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

        GameManager.Instance.ClaimRespawnPoimt(this.gameObject);
    }
}
