using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

//Creates the tiles that adjust based on surrounding tiles
public class AdjustableTile : Tile {
	public string tileType;

	[SerializeField]
	private Sprite[] tileSprites;

	[SerializeField]
	private Sprite tilePreview;

	public override void RefreshTile (Vector3Int position, ITilemap tilemap)
	{
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				
				//gets the neighbor tile position
				Vector3Int neighborPos = new Vector3Int (position.x + x, position.y + y, position.z);

				if (isSameTile (tilemap, neighborPos)) {
					tilemap.RefreshTile (neighborPos); 
				}
			}
		}
			
	}

	public override void GetTileData (Vector3Int p_position, ITilemap p_tilemap, ref TileData p_tileData)
	{
		string nTiles = string.Empty;

        //Needs this to activate the colliders
        base.GetTileData(p_position, p_tilemap, ref p_tileData);

        for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if(x != 0 || y !=0 ){
				if (isSameTile (p_tilemap, new Vector3Int (p_position.x + x, p_position.y + y, p_position.z))) {
					nTiles += 'Y';
				} else {
					nTiles+= "N";
					}
				}

			}
		}




        int spriteIndex = 47;
		//The basic ones
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'Y' && nTiles[6] == 'Y') {
            spriteIndex = 0;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'Y' && nTiles[6] == 'Y') {
            spriteIndex = 1;
        }
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles[6] == 'N') {
            spriteIndex = 2;
        }
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
            spriteIndex = 3;
        }
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
            spriteIndex = 4;
        }
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
            spriteIndex = 5;
        }
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
            spriteIndex = 6;
        }
		if ( nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
            spriteIndex = 7;
        }
		if (nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'Y'&& nTiles[6] == 'N') {
            spriteIndex = 8;
        }


		//Hallway pieces
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'N' && nTiles[6] == 'Y') {
            spriteIndex = 9;
        }
		if (nTiles [1] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'N' && nTiles[6] == 'Y') {
            spriteIndex = 10;
        }
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles[6] == 'N') {
            spriteIndex = 11;
        }
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
            spriteIndex = 12;
        }
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'N' && nTiles[6] == 'N') {
            spriteIndex = 13;
        }
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
            spriteIndex = 14;
        }
		if (nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
            spriteIndex = 15;
        }



		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [6] == 'N') {
            spriteIndex = 16;

        }
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'N'&& nTiles [6] == 'Y') {
            spriteIndex = 17;

        }
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
            spriteIndex = 18;

        }
		if (nTiles [1] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'N' && nTiles [6] == 'N') {
            spriteIndex = 19;

        }
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles [6] == 'N') {
			spriteIndex = 20;
		}



		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'N' && nTiles[6] == 'Y') {
			spriteIndex = 21;
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 22;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
            spriteIndex = 23;
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 24;
		}


		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 25;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 26;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 27;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 28;
		}




		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 29;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 30;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 31;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 32;
		}



		if (nTiles [1] == 'N'&& nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 33;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y'&& nTiles[6] == 'N') {
			spriteIndex = 34;
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 35;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y'&& nTiles[6] == 'N') {
			spriteIndex = 36;
		}



		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'N' && nTiles[6] == 'Y') {
			spriteIndex = 37;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'Y' && nTiles[6] == 'Y') {
			spriteIndex = 38;
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 39;
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 40;
		}




		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
            spriteIndex = 41;
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 42;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 43;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 44;
		}





		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			spriteIndex = 45;
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			spriteIndex = 46;
		}

        if (tileSprites[spriteIndex] == null)
        {
            p_tileData.sprite = null;
        }
        else
        {
            p_tileData.sprite = tileSprites[spriteIndex];
        }

    }

	private bool isSameTile(ITilemap tMap, Vector3Int pos){
		//returns true if it is the same tilemap type
		return tMap.GetTile (pos) == this;
	}


	//Only appears in unity editor
	#if UNITY_EDITOR

	//creates a cliclabke thing to create this scriptable object

	//found under Assets, create, tiles, and there
	[MenuItem("Assets/Create/Tiles/AdjustableTile")]


	//Saves it in the porject?
	public static void CreateAdjustableTile(){
		string path = EditorUtility.SaveFilePanelInProject ("Save AdjustableTile", "New AdjustableTile", "asset", "Save AdjustableTile", "Assets");

		if (path == "") {
			return;
		}
		//When clicked, creates a new scriptable object
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<AdjustableTile> (), path);
	}


	#endif
}
