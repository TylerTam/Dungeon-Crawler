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

		p_tileData.sprite = tileSprites [47];



		//The basic ones
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'Y' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [0];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'Y' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [1];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [2];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [3];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [4];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [5];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [6];
		}
		if ( nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [7];
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'Y'&& nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [8];
		}


		//Hallway pieces
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'N' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [9];
		}
		if (nTiles [1] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'N' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [10];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [11];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [12];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'N' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [13];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [14];
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [15];
		}



		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [6] == 'N') {
			p_tileData.sprite = tileSprites [16];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'N'&& nTiles [6] == 'Y') {
			p_tileData.sprite = tileSprites [17];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [18];
		}
		if (nTiles [1] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'N' && nTiles [6] == 'N') {
			p_tileData.sprite = tileSprites [19];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles [6] == 'N') {
			p_tileData.sprite = tileSprites [20];
		}



		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'N' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [21];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [22];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [23];
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [24];
		}


		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [25];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [26];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [27];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [28];
		}




		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [29];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [30];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [31];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [32];
		}



		if (nTiles [1] == 'N'&& nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [33];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y'&& nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [34];
		}
		if (nTiles [1] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [35];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y'&& nTiles[6] == 'N') {
			p_tileData.sprite = tileSprites [36];
		}



		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'N' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [37];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'N' && nTiles [5] == 'Y' && nTiles[6] == 'Y') {
			p_tileData.sprite = tileSprites [38];
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [39];
		}
		if (nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'N' && nTiles [4] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [40];
		}




		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [41];
		}
		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [42];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [43];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [44];
		}





		if (nTiles [0] == 'Y' && nTiles [1] == 'Y' && nTiles [2] == 'N' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'N' && nTiles[6] == 'Y' && nTiles [7] == 'Y') {
			p_tileData.sprite = tileSprites [45];
		}
		if (nTiles [0] == 'N' && nTiles [1] == 'Y' && nTiles [2] == 'Y' && nTiles [3] == 'Y' && nTiles [4] == 'Y' && nTiles [5] == 'Y' && nTiles[6] == 'Y' && nTiles [7] == 'N') {
			p_tileData.sprite = tileSprites [46];
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
