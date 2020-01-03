namespace UnitTests
{
    using System;
    using System.Threading.Tasks;
    using Cache;
    using DeadSimpleCache;
    using FluentAssertions;
    using Xunit;

    public class SimpleCache_Tests
    {
        [Fact]
        public void Set_Get_Object()
        {
            var sut = new SimpleCache();
            var key = "key";
            var num = 1;
            var name = "name";
            var obj = new SampleCacheObject { Name = name, Number = num };
            sut.Set(key, obj);
            var result = sut.Get<SampleCacheObject>(key);
            result.Should().NotBeNull();
            result.ValueOrDefault.Should().NotBeNull();
            result.IsNull.Should().Be(false);
            result.ValueOrDefault.Number.Should().Be(num);
            result.ValueOrDefault.Name.Should().Be(name);
        }

        [Fact]
        public void Set_Get_String()
        {
            var sut = new SimpleCache();
            var key = "key";
            var val = "name";
            sut.Set(key, val);
            var result = sut.Get<string>(key);
            result.Should().NotBeNull();
            result.ValueOrDefault.Should().NotBeNull();
            result.IsNull.Should().Be(false);
            result.ValueOrDefault.Should().Be(val);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Set_Get_Bool(bool val)
        {
            var sut = new SimpleCache();
            var key = "key";
            sut.Set(key, val);
            var result = sut.Get<bool>(key);
            result.Should().NotBeNull();
            result.IsNull.Should().Be(false);
            result.ValueOrDefault.Should().Be(val);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void Set_Get_Bool_Null(bool? val)
        {
            var sut = new SimpleCache();
            var key = "key";
            sut.Set(key, val);
            var result = sut.Get<bool?>(key);
            result.Should().NotBeNull();
            result.IsNull.Should().Be(val == null);
            result.ValueOrDefault.Should().Be(val);
        }

        [Theory]
        [InlineData("string")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetCacheThenSourceAsync_String_FromSource(string val)
        {
            var sut = new SimpleCache();
            var key = "key";
            var result = await sut.GetCacheThenSourceAsync<string>(
                key: key,
                sourceDelegate: () => Task.FromResult(val));
            result.IsNull.Should().Be(val == null);
            result.ValueOrDefault.Should().Be(val);
        }

        [Fact]
        public async Task GetCacheThenSourceAsync_Object_FromSource()
        {
            var sut = new SimpleCache();
            var key = "key";
            var name = "name";
            var num = 2;
            var obj = new SampleCacheObject { Name = name, Number = num };
            var result = await sut.GetCacheThenSourceAsync<SampleCacheObject>(
                key: key,
                sourceDelegate: () => Task.FromResult(obj));
            result.IsNull.Should().Be(false);
            result.ValueOrDefault.Should().NotBeNull();
            result.ValueOrDefault.Number.Should().Be(num);
            result.ValueOrDefault.Name.Should().Be(name);

            var cachePersisted = sut.Get<SampleCacheObject>(key);
            cachePersisted.IsNull.Should().BeFalse();
            cachePersisted.ValueOrDefault.Number.Should().Be(num);
            cachePersisted.ValueOrDefault.Name.Should().Be(name);
        }

        [Fact]
        public async Task GetCacheThenSourceAsync_Object_FromCache()
        {
            var sut = new SimpleCache();
            var key = "key";
            var name = "name";
            var num = 2;
            var obj = new SampleCacheObject { Name = name, Number = num };
            sut.Set(key, obj);
            var didSourceRun = false;
            var source = new Func<Task<SampleCacheObject>>(() =>
            {
                didSourceRun = true;
                return Task.FromResult(obj);
            });
            var result = await sut.GetCacheThenSourceAsync<SampleCacheObject>(
                key: key,
                sourceDelegate: source);
            result.IsNull.Should().Be(false);
            result.ValueOrDefault.Should().NotBeNull();
            result.ValueOrDefault.Number.Should().Be(num);
            result.ValueOrDefault.Name.Should().Be(name);
            didSourceRun.Should().BeFalse();
        }
    }
}

