using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleField : MonoBehaviour
{
    public GameObject bubbleBlue;
    public GameObject bubbleRed;
    public GameObject bubbleGreen;
    public GameObject bubblePurple;
    public GameObject bubbleMulti;
    public GameObject bubbleHeavy;
    public GameObject bubbleBomb;
    public GameObject coin;
    public AudioClip[] soundsBubbleGood;
    public AudioClip[] soundsBubbleShow;
    public AudioClip[] soundsBubbleBad;
    Magnet magnet;
    int level;
    int counter;
    int currentSize;
    int currentColor;
    int currentBubbleMaxCount;
    GameObject[] coins = new GameObject[12];

    void Awake()
    {
        DefsGame.bubbleField = this;
    }

    // Use this for initialization
    void Start()
    {
        magnet = GetComponentInChildren<Magnet>();
    }

    public void Init()
    {
        level = 0;
        currentSize = 5;
        currentColor = DefsGame.BUBBLE_COLOR_ONE;
        currentBubbleMaxCount = 1;
        magnet.Init();
        AddFirst();
    }

    public void HideCoins()
    {
        GameObject _object;
        for (int i = 0; i < coins.Length; i++)
        {
            _object = coins[i];
            if (_object)
            {
                Coin coinScript = _object.GetComponent<Coin>();
                coinScript.Hide(true);
            }
        }
    }

    public void Hide()
    {
        HideCoins();
        magnet.Hide();
    }

    void AddFirst()
    {
        GameObject _newBubble = (GameObject) Instantiate(bubbleBlue, transform.position, Quaternion.identity);
        magnet.addToEmptyPlace(_newBubble);
        counter = 1;
    }

    void MakeNewBubble()
    {
        GameObject _gameObjectBubble = TryToCreateNewBubble();
        if (_gameObjectBubble)
        {
            ++counter;
            Bubble _bubble = _gameObjectBubble.GetComponent<Bubble>();


            if (((counter == 5) || ((counter > 7) && (counter + 5) % 7 == 0))
                && ((_bubble.id != DefsGame.BUBBLE_COLOR_TIMER) && (_bubble.id != DefsGame.BUBBLE_COLOR_HEAVY)))
            {
                GameObject _coin = (GameObject) Instantiate(coin, transform.position, Quaternion.identity);
                Coin coinScript = _coin.GetComponent<Coin>();
                coinScript.parentObj = _gameObjectBubble;
                coinScript.Show(true);
                addCoinToEmptyPlace(_coin);
                _bubble.SetCoin(coinScript);
                //	trace("bonusManager.add(_obj);");
            }
        }
    }

    public void addCoinToEmptyPlace(GameObject _objectNew)
    {
        GameObject _object;
        for (int i = 0; i < coins.Length; i++)
        {
            _object = coins[i];
            if (_object == null)
            {
                coins[i] = _objectNew;
                return;
            }
        }
    }

    public void NextLevel()
    {
        ++level;
        if (level == 1)
        {
            currentBubbleMaxCount = 2;
        }
        else if (level <= 3)
        {
            currentBubbleMaxCount = 3;
        }
        else if (level == 8)
        {
            currentBubbleMaxCount = 4;
        }
        else if (level == 12)
        {
            currentBubbleMaxCount = 5;
        }
        else if (level == 24)
        {
            currentBubbleMaxCount = 6;
        }
        else if (level == 27)
        {
            currentBubbleMaxCount = 7;
        }
        else if (level == 30)
        {
            currentBubbleMaxCount = 8;
        }
        else if (level == 35)
        {
            currentBubbleMaxCount = 9;
        }
//        else if (level == 40)
//        {
//            currentBubbleMaxCount = 10;
//        }

        int _cnt = magnet.GetBubblesType(-1, true);
        for (int i = _cnt; i < currentBubbleMaxCount; i++)
        {
            MakeNewBubble();
        }
    }

    GameObject TryToCreateNewBubble()
    {
        bool _cantCreate = true;
        int _cnt = 0;
        Vector2 _pos = new Vector2();

        int _colorID = ChooseColor();
        float _newScale = currentSize / 5f;

        while ((_cantCreate) && (_cnt < 500))
        {
            _pos = new Vector2(
                transform.position.x + Random.Range(-magnet.radius * 1.15f, Random.value * magnet.radius * 1.15f),
                transform.position.y + Random.Range(-magnet.radius * 1.1f, Random.value * magnet.radius * 1.1f));
            if (!magnet.CheckRadius(_pos, _newScale * 1.6f * 0.4f))
            {
                _cantCreate = false;
                break;
            }
            ++_cnt;
        }

        if (!_cantCreate)
        {
            GameObject _newGameObject = null;
            switch (_colorID)
            {
                case DefsGame.BUBBLE_COLOR_ONE:
                    _newGameObject = (GameObject) Instantiate(bubbleBlue, _pos, Quaternion.identity);
                    break;
                case DefsGame.BUBBLE_COLOR_TWO:
                    _newGameObject = (GameObject) Instantiate(bubbleRed, _pos, Quaternion.identity);
                    break;
                case DefsGame.BUBBLE_COLOR_THREE:
                    _newGameObject = (GameObject) Instantiate(bubbleGreen, _pos, Quaternion.identity);
                    break;
                case DefsGame.BUBBLE_COLOR_FOUR:
                    _newGameObject = (GameObject) Instantiate(bubblePurple, _pos, Quaternion.identity);
                    break;
                case DefsGame.BUBBLE_COLOR_MULTI:
                    _newGameObject = (GameObject) Instantiate(bubbleMulti, _pos, Quaternion.identity);
                    break;
                case DefsGame.BUBBLE_COLOR_HEAVY:
                    _newGameObject = (GameObject) Instantiate(bubbleHeavy, _pos, Quaternion.identity);
                    break;
                case DefsGame.BUBBLE_COLOR_TIMER:
                    _newGameObject = (GameObject) Instantiate(bubbleBomb, _pos, Quaternion.identity);
                    break;
            }

            magnet.addToEmptyPlace(_newGameObject);
            Bubble _bubble = _newGameObject.GetComponent<Bubble>();
            _bubble.SetStartSize(currentSize);
            return _newGameObject;
        }
        else
            return null;
    }

    int ChooseColor()
    {
        float _arrTimersLength = magnet.GetBubblesType(DefsGame.BUBBLE_COLOR_TIMER, true);
        float _arrHeavyLength = magnet.GetBubblesType(DefsGame.BUBBLE_COLOR_HEAVY, true);
        float _arrAllLength = magnet.GetBubblesType(-1, true);
        float _ran = Random.value;
        float _ranSize = Random.value;

        if (level <= 0)
        {
            currentColor = DefsGame.BUBBLE_COLOR_ONE;
            currentSize = 5;
        }
        else if (level <= 3)
        {
//
            if (_ran <= 0.5f) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else
                currentColor = DefsGame.BUBBLE_COLOR_TWO;

            /*if ((_ran <= 0.15f)&&(_arrAllLength > _arrTimersLength+_arrHeavyLength)) currentColor = DefsGame.BUBBLE_COLOR_TIMER; else
                if ((_ran <= 0.3f)&&(_arrAllLength > _arrTimersLength+_arrHeavyLength)) currentColor = DefsGame.BUBBLE_COLOR_HEAVY; else
                    if (_ran <= 0.46f) currentColor = DefsGame.BUBBLE_COLOR_ONE; else
                        if (_ran <= 0.62f) currentColor = DefsGame.BUBBLE_COLOR_TWO; else 
                            if (_ran <= 0.78f) currentColor = DefsGame.BUBBLE_COLOR_THREE; else 
                                if (_ran <= 0.94f) currentColor = DefsGame.BUBBLE_COLOR_FOUR; else 
                                    currentColor = DefsGame.BUBBLE_COLOR_MULTI;*/

            currentSize = 5;
        }
        else if (level <= 6)
        {
            if (_ran <= 0.4f) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.8f) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;
            currentSize = 4 + Mathf.RoundToInt(Random.value * (5f - 4f));
        }
        else if (level <= 10)
        {
            if (_ran <= 0.25f) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.50f) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.75f) currentColor = DefsGame.BUBBLE_COLOR_THREE;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;
            currentSize = 3 + Mathf.RoundToInt(Random.value * (5f - 4f));
        }
        else if (level <= 13)
        {
            if ((_ran <= 0.10f) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_TIMER;
            else if (_ran <= 0.30f) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.50f) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.70f) currentColor = DefsGame.BUBBLE_COLOR_THREE;
            else if (_ran <= 0.90f) currentColor = DefsGame.BUBBLE_COLOR_FOUR;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;

            if (_ranSize < 0.4f)
                currentSize = 3;
            else if (_ranSize < 0.75f)
                currentSize = 4;
            else
                currentSize = 5;
        }
        else if (level < 17)
        {
            if ((_ran <= 0.2) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_TIMER;
            else if ((_ran <= 0.3) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_HEAVY;
            else if (_ran <= 0.45) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.6) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.75) currentColor = DefsGame.BUBBLE_COLOR_THREE;
            else if (_ran <= 0.9) currentColor = DefsGame.BUBBLE_COLOR_FOUR;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;
            if (_ranSize < 0.4f)
                currentSize = 3;
            else if (_ranSize < 0.8f)
                currentSize = 4;
            else
                currentSize = 5;
        }
        else if (level < 25)
        {
            if ((_ran <= 0.15) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_TIMER;
            else if ((_ran <= 0.3) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_HEAVY;
            else if (_ran <= 0.45) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.6) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.75) currentColor = DefsGame.BUBBLE_COLOR_THREE;
            else if (_ran <= 0.9) currentColor = DefsGame.BUBBLE_COLOR_FOUR;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;
            
            if (_ranSize < 0.3333f)
                currentSize = 2;
            if (_ranSize < 0.6111f)
                currentSize = 3;
            else if (_ranSize < 0.85f)
                currentSize = 4;
            else
                currentSize = 5;
            if ((currentColor == DefsGame.BUBBLE_COLOR_TIMER) && (currentSize > 3))
            {
                currentSize -= 2;
            }
        }
        else if (level < 40)
        {
            if ((_ran <= 0.15f) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_TIMER;
            else if ((_ran <= 0.3f) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_HEAVY;
            else if (_ran <= 0.45f) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.6f) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.75f) currentColor = DefsGame.BUBBLE_COLOR_THREE;
            else if (_ran <= 0.9f) currentColor = DefsGame.BUBBLE_COLOR_FOUR;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;
            if (_ranSize < 0.35f)
                currentSize = 2;
            if (_ranSize < 0.622f)
                currentSize = 3;
            else if (_ranSize < 0.87f)
                currentSize = 4;
            else
                currentSize = 5;

            if ((currentColor == DefsGame.BUBBLE_COLOR_TIMER) && (currentSize > 3))
            {
                currentSize -= 2;
            }
        }
        else
        {
            if ((_ran <= 0.15f) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_TIMER;
            else if ((_ran <= 0.3f) && (_arrAllLength > _arrTimersLength + _arrHeavyLength))
                currentColor = DefsGame.BUBBLE_COLOR_HEAVY;
            else if (_ran <= 0.46f) currentColor = DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.62f) currentColor = DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.78f) currentColor = DefsGame.BUBBLE_COLOR_THREE;
            else if (_ran <= 0.94f) currentColor = DefsGame.BUBBLE_COLOR_FOUR;
            else
                currentColor = DefsGame.BUBBLE_COLOR_MULTI;
            
            if (_ranSize < 0.5f)
                currentSize = 2;
            if (_ranSize < 0.8f)
                currentSize = 3;
            else if (_ranSize < 0.95f)
                currentSize = 4;
            else
                currentSize = 5;

            if ((currentColor == DefsGame.BUBBLE_COLOR_TIMER) && (currentSize > 3))
            {
                currentSize -= 2;
            }
        }

        if (currentColor == DefsGame.BUBBLE_COLOR_HEAVY)
        {
            currentSize = 4 + Mathf.RoundToInt(Random.value * (5f - 4f));
        }

        return currentColor;
    }


    public int GetRandomColor()
    {
        if ((level != 0) && (level % 10 == 0)) return DefsGame.BUBBLE_COLOR_MULTI;

        int _bubbleArrLength = magnet.bubbleMaxCount;
        GameObject _obj = null;
        Bubble _bubble = null;

        List<int> _list = new List<int>();
        for (int i = 0; i < _bubbleArrLength; i++)
        {
            _obj = magnet.GetObject(i);
            if (_obj)
            {
                _bubble = _obj.GetComponent<Bubble>();
                if ((!_bubble.isDoneAnimation)
                    && (_bubble.id != DefsGame.BUBBLE_COLOR_TIMER) && (_bubble.id != DefsGame.BUBBLE_COLOR_HEAVY))
                {
                    _list.Add(_bubble.id);
                }
            }
        }

        int _id = Mathf.RoundToInt(Random.value * (_list.Count - 1));
        int _colorID = _list[_id];

        if (_colorID == DefsGame.BUBBLE_COLOR_MULTI)
        {
            float _ran = Random.value;
            if (_ran <= 0.25f) return DefsGame.BUBBLE_COLOR_ONE;
            else if (_ran <= 0.5f) return DefsGame.BUBBLE_COLOR_TWO;
            else if (_ran <= 0.75f) return DefsGame.BUBBLE_COLOR_THREE;
            else
                return DefsGame.BUBBLE_COLOR_FOUR;
        }

        return _colorID;
    }

    public AudioClip GetSoundShowClip(int _bubbleID)
    {
        if ((_bubbleID == DefsGame.BUBBLE_COLOR_ONE)
            || (_bubbleID == DefsGame.BUBBLE_COLOR_TWO)
            || (_bubbleID == DefsGame.BUBBLE_COLOR_THREE)
            || (_bubbleID == DefsGame.BUBBLE_COLOR_FOUR))
        {
            float _ran = Random.value;
            if (_ran <= 0.25f) return soundsBubbleShow[0];
            else if (_ran <= 0.5f) return soundsBubbleShow[1];
            else if (_ran <= 0.75f) return soundsBubbleShow[2];
            else
                return soundsBubbleShow[3];
        }

        return soundsBubbleShow[4];

        /*switch (_bubbleID)  {
            case DefsGame.BUBBLE_COLOR_ONE: 
                return soundsBubbleShow [0];
                break;
            case DefsGame.BUBBLE_COLOR_TWO:
            return soundsBubbleShow [1];
                break;
            case DefsGame.BUBBLE_COLOR_THREE:
            return soundsBubbleShow [2];
                break;
            case DefsGame.BUBBLE_COLOR_FOUR:
            return soundsBubbleShow [3];
                break;
            case DefsGame.BUBBLE_COLOR_MULTI:
            return soundsBubbleShow [4];
                break;
            case DefsGame.BUBBLE_COLOR_TIMER:
            return soundsBubbleShow [4];
                break;
            case DefsGame.BUBBLE_COLOR_HEAVY:
                return soundsBubbleGood [4];
                break;

        }
        return soundsBubbleGood [0];*/
    }

    public AudioClip GetSoundBadClip(int _bubbleID)
    {
        switch (_bubbleID)
        {
            case DefsGame.BUBBLE_COLOR_ONE:
                return soundsBubbleBad[0];
            case DefsGame.BUBBLE_COLOR_TWO:
                return soundsBubbleBad[1];
            case DefsGame.BUBBLE_COLOR_THREE:
                return soundsBubbleBad[2];
            case DefsGame.BUBBLE_COLOR_FOUR:
                return soundsBubbleBad[3];
        }
        return null;
    }

    public AudioClip GetSoundGoodClip(int _bubbleID)
    {
        switch (_bubbleID)
        {
            case DefsGame.BUBBLE_COLOR_ONE:
                return soundsBubbleGood[Mathf.RoundToInt(Random.Range(0, 2))];
            case DefsGame.BUBBLE_COLOR_TWO:
                return soundsBubbleGood[Mathf.RoundToInt(Random.Range(3, 6))];
            case DefsGame.BUBBLE_COLOR_THREE:
                return soundsBubbleGood[Mathf.RoundToInt(Random.Range(7, 10))];
            case DefsGame.BUBBLE_COLOR_FOUR:
                return soundsBubbleGood[Mathf.RoundToInt(Random.Range(11, 14))];
            case DefsGame.BUBBLE_COLOR_MULTI:
                return soundsBubbleGood[Mathf.RoundToInt(Random.Range(15, 17))];
            case DefsGame.BUBBLE_COLOR_TIMER:
                return soundsBubbleGood[Mathf.RoundToInt(Random.Range(18, 20))];
            case DefsGame.BUBBLE_COLOR_HEAVY:
                return soundsBubbleGood[21];
        }
        return soundsBubbleGood[0];
    }
}