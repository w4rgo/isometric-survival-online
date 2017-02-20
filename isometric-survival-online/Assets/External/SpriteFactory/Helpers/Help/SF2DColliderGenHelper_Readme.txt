SF2DColliderGenHelper (beta) is a component to help you use 2D Collider Gen with Sprite Factory.

To create a mesh collider on a Sprite:
1. Add an AlphaMeshCollider from 2D Collider Gen from the 2D Collider Gen menu.
2. Add the included SF2DColliderGenHelper component to the Sprite.
3. Click the "Rebuild 2D Collider Gen Collider" collider button in the inspector to rebuild the mesh collider matching the size and shape of your Sprite.

There are certain circumstances which may cause the mesh collider to no longer line up with your Sprite. In these cases, you must click "Rebuild 2D Collider Gen Collider" on the affected Sprite(s). The circumstances where you may need to rebuild the colliders are as follows:

1. You change the frame graphics.
2. You offset the frame in the editor.
3. You change the default animation to a different one or delete the default animation.
4. You add or remove frames from the Sprite or Sprite Group and the atlas size expans or contracts.
5. You change the max atlas size in the editor and it rebuilds the atlases at a different size than before.

Please Note:
2D Collider Gen can only generate a single mesh collider for the first frame of a sprite. There is no support for animation of mesh colliders in 2D Collider Gen. Because of this, mesh colliders are best used with static Sprites (sprites with on 1 frame of animation).