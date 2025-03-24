using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public GameObject RoomTrigger;
    public GameObject EntranceDoor;
    public GameObject ExitDoor;
}

public class SectionReset : MonoBehaviour
{
    public List<Room> RoomList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            foreach(Room room in RoomList)
            {
                if(room.RoomTrigger != null) 
                { 
                    BoxCollider boxCollider = room.RoomTrigger.GetComponent<BoxCollider>();
                    if(boxCollider != null)
                    {
                        boxCollider.enabled = true;
                    }
                }

                if(room.EntranceDoor != null)
                {
                    room.EntranceDoor.SetActive(false);
                }

                if (room.ExitDoor != null)
                {
                    room.ExitDoor.SetActive(true);
                }
            }
        }
    }
}
