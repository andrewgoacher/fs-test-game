module Texture

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content

type TextureAtlasItem = {ref:int;source:Rectangle}
type TextureAtlas = {texture:Texture2D;items:TextureAtlasItem list}
type AtlasSprite = {textureAtlas:TextureAtlas;frame:int;bounds:Rectangle}
type StaticSprite = {texture:Texture2D;bounds:Rectangle}

type Sprite =
    | StaticSprite of StaticSprite
    | AtlasSprite of AtlasSprite

let createTexture name (content:ContentManager)=
    content.Load<Texture2D>(name)

let createSprite texture bounds =
    {texture=texture;bounds=bounds}

let createAtlas (texture:Texture2D) (atlasSize:(int*int)) =
    let width = texture.Width
    let height = texture.Height

    let atlasSizeX,atlasSizeY = atlasSize
    let hCount = width/atlasSizeX
    let vCount = height/atlasSizeY

    let rec createAtlasItem (list:TextureAtlasItem list) frameRef (atlasSize:(int*int)) current count =
        let xCur,yCur = current
        let xCount,yCount = count
        let atlasSizeX,atlasSizeY = atlasSize
        let rect = new Rectangle(xCur*atlasSizeX, yCur*atlasSizeY, atlasSizeX, atlasSizeY)
        let newItem = {ref=frameRef;source = rect}
        let newList = list@[newItem]

        match current with
        | x,y when x = (xCount-1) && y = (yCount-1) -> list
        | x,y when x = xCount-1 ->
            createAtlasItem newList (frameRef+1) atlasSize (0,y+1) count
        | x,y ->
            createAtlasItem newList (frameRef+1) atlasSize (x+1,y) count
    
    let textureAtlas = createAtlasItem [] 0 atlasSize (0,0) (hCount,vCount)
    {texture=texture;items=textureAtlas}

let createAtlasSprite textureAtlas frame bounds =
    {textureAtlas=textureAtlas;bounds=bounds;frame=frame}

let texture sprite =
    match sprite with
    | StaticSprite s -> s.texture
    | AtlasSprite a -> a.textureAtlas.texture

let bounds sprite =
    match sprite with
    | StaticSprite s -> s.bounds
    | AtlasSprite a -> a.bounds

let source sprite =
    match sprite with 
    | StaticSprite s -> s.bounds
    | AtlasSprite a -> 
        let items = a.textureAtlas.items
        items.[a.frame].source

let draw (spritebatch:SpriteBatch) sprite rect =
    let texture = texture sprite
    let bounds = 
        match rect with
        | None -> (bounds sprite)
        | Some b -> b

    let source = System.Nullable <| source sprite

    spritebatch.Draw(
        texture,
        bounds,
        source,
        Color.White)