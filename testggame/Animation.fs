module Animation

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Texture

type Frame = {atlasItem:AtlasSprite;frameDuration:float}
type Animation = {name:string;frames:Frame list;loops:bool}
type Animations = {animations:Animation list;currentAnimation:string}
type AnimationState = {
    animations:Animations
    position:Vector2
    frameTime:float
    active:bool
    currentFrame:int
}

let createFrame sprite duration =
    {atlasItem=sprite;frameDuration=duration}

let createAnimation name frames loops =
    {name=name;frames=frames;loops=loops}

let createAnimationFromAtlas name atlas indices duration loops =
    let frames = 
        indices
        |> List.map (fun index ->
            let sprite =Texture.createAtlasSprite atlas index atlas.items.[index].source
            createFrame sprite duration
        )
    createAnimation name frames loops 

let addAnimation (animations:Animations) (animation :Animation)=
    let currentAnimations = animations.animations
    match currentAnimations with
    | [] -> {animations=[animation];currentAnimation=animation.name}
    | list -> {animations=animation::list;currentAnimation=animations.currentAnimation}

let createAnimationState animations position frameTime isActive currentFrame =
    {animations=animations;position=position;frameTime=frameTime;active=isActive;currentFrame=currentFrame}

let isActive animationState =
    animationState.active

let position animationState =
    animationState.position

let currentAnimation animation =
    let animationCollection = animation.animations
    let name = animationCollection.currentAnimation
    let selectedAnimation = 
        animationCollection.animations 
        |> List.tryPick (fun anim ->
           if anim.name.Equals(name) then
               Some anim
           else None)

    match selectedAnimation with
    | Some a -> a
    | None -> failwith "not found"

let frame animationState animation =
    let frames = animation.frames
    let frameRef = animationState.currentFrame
    frames.[frameRef]

let draw (spritebatch:SpriteBatch) (currentState:AnimationState) =
   if not <| isActive currentState then
    ()
   else
        let position = position currentState
        let currentAnimation = currentAnimation currentState
        let frame = frame currentState currentAnimation
        let source = Texture.source <| Texture.AtlasSprite frame.atlasItem
        let bounds = Math.createRect (int position.X) (int position.Y) source.Width source.Height

        Texture.draw spritebatch (Texture.AtlasSprite frame.atlasItem) (Some bounds)

let update (gameTime:GameTime) (currentState:AnimationState) =
    currentState
    //match isActive currentState with
    //| false -> ()
    //| true ->
    //    let elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds
    //    let currentAnimation = currentAnimation currentState
    //    let currentFrame = getFrame currentState
    //    let newFrameTime = (frameTime animation) + elapsedTime
    //    let duration = currentFrame.frameDuration


    //if animation.stopped then
    //    animation
    //else 

    //    let currentFrame = currentFrame animation

    //    if newFrameTime > duration then
    //        let newFrameRef = currentFrame.atlasRef + 1
    //        let totalFrameCount = frameCount animation
    //        if newFrameRef >= totalFrameCount then
    //            if animation.loops then
    //                createAnimation animation.atlas 0 0.0 animation.frameDuration animation.loops animation.position animation.stopped
    //            else
    //                createAnimation animation.atlas animation.currentFrame animation.frameTime animation.frameDuration animation.loops animation.position true
    //        else
    //            createAnimation animation.atlas (animation.currentFrame + 1) 0.0 animation.frameDuration animation.loops animation.position animation.stopped
    //    else 
    //        createAnimation animation.atlas animation.currentFrame newFrameTime animation.frameDuration animation.loops animation.position animation.stopped
