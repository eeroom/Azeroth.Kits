using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharpcore
{
    class GenricUtil
    {
        static void Main(string[] args)
        {
            /**
            逆变
            ProcessHandlerIn<People>和ProcessHandlerIn<Animal>没有继承关系
            People是Animal的子类
            ProcessHandlerIn<People>的变量指向ProcessHandlerIn<Animal>的实例，形式上类似于：子类的变量指向父类的实例，所以是逆变
            因为类型参数仅作用于参数位置，当子类变量调用方法的时候，传入的参数值类型要求是子类型，而变量实际的参数值类型是父类型，所以调用肯定没问题
            参数值仍然遵循父类的变量指向子类的实例
            */
            ProcessHandlerIn<Animal> phpin = null;
            ProcessHandlerIn<People> phs = phpin;

            /**
            协变
            ProcessHandlerOut<People>ProcessHandlerOut<Student>没有继承关系
            People是Student的父类
            ProcessHandlerOut<People>的变量指向ProcessHandlerOut<Student>的实例，形式上类似于：父类的变量指向子类的实例，所以是协变
            因为类型参数仅作用于返回值位置，当父类变量调用方法的时候，返回值类型要求是父类型，而变量实际返回值得类型是子类型，所以调用肯定没问题
            返回值仍然遵循父类的变量指向子类的实例
            */
            ProcessHandlerOut<Student> phpout = null;
            ProcessHandlerOut<People> pha = phpout;

            /**
             总结：
             定义的方法中存在泛型参数，
             如果方法中，泛型的类型参数仅作用于返回值位置，则可以把类型由<T>调整为<? extends T>
             如果方法中，泛型的类型参数仅作用于参数位置，则可以把类型由<T>调整为<? super T>
             java的泛型类型参数既能逆变，也能协变，支持泛型类和泛型接口，由调用者决定，编译器根据调用者的代码是否符合父类变量指向子类实例这一规则，判定是否允许编译通过
             c#的泛型类型参数只能逆变或协变或者不可变，支持泛型接口和泛型委托，由类型定义者决定，in和out关键字已经提前确保调用者的代码一定符合父类变量指向子类实例这一规则
            */

        }
    }
}
