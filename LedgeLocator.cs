using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeLocator : Character
{
    public AnimationClip clip;
    public float climbingHorizontalOffset;

    private Vector2 topOfPlayer;
    private GameObject ledge;
    private bool falling;
    private bool moved;

    protected virtual void FixedUpdate()
    {
        CheckForLedge();
        LedgeHanging();
    }

    protected virtual void CheckForLedge()
    {
        if (!falling)
        {
            if (!character.isFacingLeft)
            {
                topOfPlayer = new Vector2(col.bounds.max.x + .1f, col.bounds.max.y);
                RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.right, .2f);
                if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                {
                    ledge = hit.collider.gameObject;
                    if (col.bounds.max.y < ledge.GetComponent<Collider2D>().bounds.max.y && col.bounds.max.y > ledge.GetComponent<Collider2D>().bounds.center.y)
                    {
                        character.grabbingLedge = true;
                        anim.SetBool("LedgeHanging", true);
                    }
                }
            }
            else
            {
                topOfPlayer = new Vector2(col.bounds.min.x - .1f, col.bounds.max.y);
                RaycastHit2D hit = Physics2D.Raycast(topOfPlayer, Vector2.left, .2f);
                if (hit && hit.collider.gameObject.GetComponent<Ledge>())
                {
                    ledge = hit.collider.gameObject;
                    if (col.bounds.max.y < ledge.GetComponent<Collider2D>().bounds.max.y && col.bounds.max.y > ledge.GetComponent<Collider2D>().bounds.center.y)
                    {
                        anim.SetBool("LedgeHanging", true);
                        character.grabbingLedge = true;
                    }
                }
            }
            if (ledge != null)
            {
                AdjustPlayerPosition();
            }
        }
    }

    protected virtual void LedgeHanging()
    {
        if (character.grabbingLedge && Input.GetAxis("Vertical") > 0)
        {
            anim.SetBool("LedgeHanging", false);
            if (!character.isFacingLeft)
            {
                StartCoroutine(ClimbingLedge(new Vector2(transform.position.x + climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y + col.bounds.extents.y), clip.length - .3f));
            }
            else
            {
                StartCoroutine(ClimbingLedge(new Vector2(transform.position.x - climbingHorizontalOffset, ledge.GetComponent<Collider2D>().bounds.max.y + col.bounds.extents.y), clip.length - .3f));
            }
        }
        if (character.grabbingLedge && Input.GetAxis("Vertical") < 0)
        {
            ledge = null;
            moved = false;
            character.grabbingLedge = false;
            anim.SetBool("LedgeHanging", false);
            falling = true;
            Invoke("NotFalling", .5f);
        }
    }

    protected virtual IEnumerator ClimbingLedge(Vector2 topOfPlatform, float duration)
    {
        float time = 0;
        Vector2 startValue = transform.position;
        while (time < duration)
        {
            anim.SetBool("LedgeClimbing", true);
            transform.position = Vector2.Lerp(startValue, topOfPlatform, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        ledge = null;
        moved = false;
        character.grabbingLedge = false;
        anim.SetBool("LedgeClimbing", false);
    }

    protected virtual void AdjustPlayerPosition()
    {
        if (!moved)
        {
            moved = true;
            transform.position = new Vector2(transform.position.x + ledge.GetComponent<Ledge>().hangingHorizontalOffset, transform.position.y + ledge.GetComponent<Ledge>().hangingVerticalOffset);
        }
    }

    protected virtual void NotFalling()
    {
        falling = false;
    }
}
