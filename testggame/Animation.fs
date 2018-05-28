module Animation

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type AtlasItem = {frameRef:int;sourceRect:Rectangle}
type TextureAtlas = {texture:Texture2D;textureAtlas:AtlasItem list}

type Animation = {
    atlas:TextureAtlas
    currentFrame: int
    frameTime: float
    frameDuration: float
    loops: bool
    position: Vector2
    stopped:bool
}

let createAnimation atlas currentFrame frameTime duration loops position stopped =
    {
        atlas = atlas
        currentFrame = currentFrame
        frameTime = frameTime
        frameDuration = duration
        loops = loops
        position = position
        stopped = stopped
    }

let sourceRect animation =
    let atlas = animation.atlas
    let textureAtlas = atlas.textureAtlas
    let frameId = animation.currentFrame
    let frame = textureAtlas.[frameId]
    frame.sourceRect

let position animation =
    animation.position

let texture animation =
    animation.atlas.texture

let createAtlas (texture:Texture2D) (atlastSize:Point) =
    let width = texture.Width
    let height = texture.Height

    let hCount = width/atlastSize.X
    let vCount = height/atlastSize.Y

    let rec createAtlasItem (list:AtlasItem list) frameRef (atlasSize:Point) current count =
        let xCur,yCur = current
        let xCount,yCount = count

        let rect = new Rectangle(xCur*atlasSize.X, yCur*atlasSize.Y, atlasSize.X, atlasSize.Y)
        let newItem = {frameRef=frameRef;sourceRect = rect}
        let newList = list@[newItem]

        match current with
        | x,y when x = (xCount-1) && y = (yCount-1) -> list
        | x,y when x = xCount-1 ->
            createAtlasItem newList (frameRef+1) atlasSize (0,y+1) count
        | x,y ->
            createAtlasItem newList (frameRef+1) atlasSize (x+1,y) count
    
    let textureAtlas = createAtlasItem [] 0 atlastSize (0,0) (hCount,vCount)
    {texture=texture;textureAtlas=textureAtlas}

let draw (spritebatch:SpriteBatch) (animation:Animation) =
    match animation.stopped with
    | true -> ()
    | false -> 
        let position = position animation
        let source = System.Nullable (sourceRect animation)

        spritebatch.Draw(
            texture animation,
            position,
            source,
            Color.White)

let currentFrame animation = 
    let ref = animation.currentFrame
    let atlas = animation.atlas
    atlas.textureAtlas.[ref]

let frameTime animation =
    animation.frameTime

let duration animation =
    animation.frameDuration

let frameCount animation =
    List.length animation.atlas.textureAtlas

let update (gameTime:GameTime) (animation:Animation) =
    if animation.stopped then
        animation
    else 

        let elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds
        let newFrameTime = (frameTime animation) + elapsedTime
        let currentFrame = currentFrame animation
        let duration = duration animation

        if newFrameTime > duration then
            let newFrameRef = currentFrame.frameRef + 1
            let totalFrameCount = frameCount animation
            if newFrameRef >= totalFrameCount then
                if animation.loops then
                    createAnimation animation.atlas 0 0.0 animation.frameDuration animation.loops animation.position animation.stopped
                else
                    createAnimation animation.atlas animation.currentFrame animation.frameTime animation.frameDuration animation.loops animation.position true
            else
                createAnimation animation.atlas (animation.currentFrame + 1) 0.0 animation.frameDuration animation.loops animation.position animation.stopped
        else 
            createAnimation animation.atlas animation.currentFrame newFrameTime animation.frameDuration animation.loops animation.position animation.stopped
